﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;

namespace WebVella.Erp.Web.Components
{
	[PageComponent(Label = "Field Hidden", Library = "WebVella", Description = "Renders Hidden Input", Version = "0.0.1", IconClass = "far fa-eye-slash")]
	public class PcFieldHidden : PcFieldBase
	{
		protected ErpRequestContext ErpRequestContext { get; set; }

		public PcFieldHidden([FromServices]ErpRequestContext coreReqCtx)
		{
			ErpRequestContext = coreReqCtx;
		}

		public class PcFieldHiddenOptions : PcFieldBaseOptions
		{
			public static PcFieldHiddenOptions CopyFromBaseOptions(PcFieldBaseOptions input)
			{
				return new PcFieldHiddenOptions
				{ 
					LabelMode = input.LabelMode,
					LabelText = input.LabelText,
					Mode = input.Mode,
					Name = input.Name
				};
			}
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

				var baseOptions = InitPcFieldBaseOptions(context);
				var instanceOptions = PcFieldHiddenOptions.CopyFromBaseOptions(baseOptions);
				if (context.Options != null)
				{
					instanceOptions = JsonConvert.DeserializeObject<PcFieldHiddenOptions>(context.Options.ToString());
				}
				var modelFieldLabel = "";
				var model = (PcFieldBaseModel)InitPcFieldBaseModel(context, instanceOptions, label: out modelFieldLabel);
				if (String.IsNullOrWhiteSpace(instanceOptions.LabelText))
				{
					instanceOptions.LabelText = modelFieldLabel;
				}

				//Implementing Inherit label mode
				ViewBag.LabelMode = instanceOptions.LabelMode;
				ViewBag.Mode = instanceOptions.Mode;

				if (instanceOptions.LabelMode == LabelRenderMode.Undefined && baseOptions.LabelMode != LabelRenderMode.Undefined)
					ViewBag.LabelMode = baseOptions.LabelMode;

				if (instanceOptions.Mode == FieldRenderMode.Undefined && baseOptions.Mode != FieldRenderMode.Undefined)
					ViewBag.Mode = baseOptions.Mode;


				var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);
				#endregion


				if (context.Mode != ComponentMode.Options && context.Mode != ComponentMode.Help) {
					model.Value = context.DataModel.GetPropertyValueByDataSource(instanceOptions.Value);
				}

				ViewBag.Options = instanceOptions;
				ViewBag.Model = model;
				ViewBag.Node = context.Node;
				ViewBag.ComponentMeta = componentMeta;
				ViewBag.RequestContext = ErpRequestContext;
				ViewBag.AppContext = ErpAppContext.Current;

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
