﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.Erp.Web.Utils;

namespace WebVella.Erp.Web.Components
{
	[PageComponent(Label = "Form", Library = "WebVella", Description = "Render a form with a Validation token", Version = "0.0.1", IconClass = "fas fa-poll-h")]
	public class PcForm : PageComponent
	{
		protected ErpRequestContext ErpRequestContext { get; set; }

		public PcForm([FromServices]ErpRequestContext coreReqCtx)
		{
			ErpRequestContext = coreReqCtx;
		}

		public class PcFormOptions
		{
			[JsonProperty(PropertyName = "id")]
			public string Id { get; set; } = "";

			[JsonProperty(PropertyName = "name")]
			public string Name { get; set; } = "form";

			[JsonProperty(PropertyName = "method")]
			public string Method { get; set; } = "post";

			[JsonProperty(PropertyName = "hook_key")]
			public string HookKey { get; set; } = "";

			[JsonProperty(PropertyName = "label_mode")]
			public LabelRenderMode LabelMode { get; set; } = LabelRenderMode.Stacked; //To be inherited

			[JsonProperty(PropertyName = "mode")]
			public FieldRenderMode Mode { get; set; } = FieldRenderMode.Form; //To be inherited

		}

		public async Task<IViewComponentResult> InvokeAsync(PageComponentContext context)
		{
			ErpPage currentPage = null;
			try
			{
				#region << Init >>
				if (context.Node == null)
				{
					return await Task.FromResult<IViewComponentResult>(Content("Error: The node Id is required to be set as query param 'nid', when requesting this component"));
				}

				var pageFromModel = context.DataModel.GetProperty("Page");
				if (pageFromModel == null)
				{
					return await Task.FromResult<IViewComponentResult>(Content("Error: PageModel cannot be null"));
				}
				else if (pageFromModel is ErpPage)
				{
					currentPage = (ErpPage)pageFromModel;
				}
				else
				{
					return await Task.FromResult<IViewComponentResult>(Content("Error: PageModel does not have Page property or it is not from ErpPage Type"));
				}

				var instanceOptions = new PcFormOptions();
				if (context.Options != null)
				{
					instanceOptions = JsonConvert.DeserializeObject<PcFormOptions>(context.Options.ToString());
					if (instanceOptions.LabelMode == LabelRenderMode.Undefined)
						instanceOptions.LabelMode = LabelRenderMode.Stacked;
					if (instanceOptions.Mode == FieldRenderMode.Undefined)
						instanceOptions.Mode = FieldRenderMode.Form;
				}

				if (String.IsNullOrWhiteSpace(instanceOptions.Id)) {
					instanceOptions.Id = "wv-" + context.Node.Id.ToString();
				}

				var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);
				#endregion


				ViewBag.Options = instanceOptions;
				ViewBag.Node = context.Node;
				ViewBag.ComponentMeta = componentMeta;
				ViewBag.RequestContext = ErpRequestContext;
				ViewBag.AppContext = ErpAppContext.Current;
				ViewBag.ComponentContext = context;
				ViewBag.GeneralHelpSection = HelpJsApiGeneralSection;

				ViewBag.LabelRenderModeOptions = ModelExtensions.GetEnumAsSelectOptions<LabelRenderMode>(); 

				ViewBag.FieldRenderModeOptions = ModelExtensions.GetEnumAsSelectOptions<FieldRenderMode>(); 


				context.Items[typeof(LabelRenderMode)] = instanceOptions.LabelMode;
				context.Items[typeof(FieldRenderMode)] = instanceOptions.Mode;

				var validation = context.DataModel.GetProperty("Validation") as ValidationException ?? new ValidationException();

				context.Items[typeof(ValidationException)] = validation;
				ViewBag.Validation = validation;

				ViewBag.Action = "";
				if (!String.IsNullOrWhiteSpace(instanceOptions.HookKey)){
					var queryList = new List<SelectOption>();
					foreach (var key in HttpContext.Request.Query.Keys) {
						if (key != "hookKey")
						{
							queryList.Add(new SelectOption(key,HttpContext.Request.Query[key].ToString()));
						}
					}
					queryList.Add(new SelectOption("hookKey",instanceOptions.HookKey)); //override even if already present

					ViewBag.Action = string.Format(HttpContext.Request.Path + "?{0}",string.Join("&", queryList.Select(kvp => string.Format("{0}={1}", kvp.Value, kvp.Label))));
				}

				ViewBag.MethodOptions = new List<SelectOption>() {
					new SelectOption("get","get"),
					new SelectOption("post","post")
				};


				switch (context.Mode)
				{
					case ComponentMode.Display:
						return await Task.FromResult<IViewComponentResult>(View("Display"));
					case ComponentMode.Design:
						return await Task.FromResult<IViewComponentResult>(View("Design"));
					case ComponentMode.Options:
						return await Task.FromResult<IViewComponentResult>(View("Options"));
					case ComponentMode.Help:
						return await Task.FromResult<IViewComponentResult>(View("Help"));
					default:
						ViewBag.ExceptionMessage = "Unknown component mode";
						ViewBag.Errors = new List<ValidationError>();
						return await Task.FromResult<IViewComponentResult>(View("Error"));
				}
			}
			catch (ValidationException ex)
			{
				ViewBag.ExceptionMessage = ex.Message;
				ViewBag.Errors = new List<ValidationError>();
				return await Task.FromResult<IViewComponentResult>(View("Error"));
			}
			catch (Exception ex)
			{
				ViewBag.ExceptionMessage = ex.Message;
				ViewBag.Errors = new List<ValidationError>();
				return await Task.FromResult<IViewComponentResult>(View("Error"));
			}
		}
	}
}
