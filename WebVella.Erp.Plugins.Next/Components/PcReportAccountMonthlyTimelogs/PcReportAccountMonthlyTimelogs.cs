﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebVella.Erp.Api;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Eql;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Plugins.Next.Utils;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.Erp.Web.Utils;

namespace WebVella.Erp.Plugins.Next.Components
{
	[PageComponent(Label = "Report: Monthly timelog for an account", Library = "WebVella", Description = "Reports for the timelog for a selected month and account", Version = "0.0.1", IconClass = "fas fa-calculator")]
	public class PcReportAccountMonthlyTimelogs : PageComponent
	{
		protected ErpRequestContext ErpRequestContext { get; set; }

		public PcReportAccountMonthlyTimelogs([FromServices]ErpRequestContext coreReqCtx)
		{
			ErpRequestContext = coreReqCtx;
		}

		public class PcReportAccountMonthlyTimelogsOptions
		{
			[JsonProperty(PropertyName = "year")]
			public string Year { get; set; } = "";

			[JsonProperty(PropertyName = "month")]
			public string Month { get; set; } = "";

			[JsonProperty(PropertyName = "account_id")]
			public string AccountId { get; set; } = "";
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

				var options = new PcReportAccountMonthlyTimelogsOptions();
				if (context.Options != null)
				{
					options = JsonConvert.DeserializeObject<PcReportAccountMonthlyTimelogsOptions>(context.Options.ToString());
				}

				var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);
				#endregion

				ViewBag.Options = options;
				ViewBag.Node = context.Node;
				ViewBag.ComponentMeta = componentMeta;
				ViewBag.RequestContext = ErpRequestContext;
				ViewBag.AppContext = ErpAppContext.Current;
				ViewBag.ComponentContext = context;

				if (context.Mode != ComponentMode.Options && context.Mode != ComponentMode.Help)
				{
					var selectedYearString = context.DataModel.GetPropertyValueByDataSource(options.Year) as string;
					var selectedMonthString = context.DataModel.GetPropertyValueByDataSource(options.Month) as string;
					var selectedAccountIdString = context.DataModel.GetPropertyValueByDataSource(options.AccountId) as string;

				}
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
						return await Task.FromResult<IViewComponentResult>(View("Error"));
				}
			}
			catch (ValidationException ex)
			{
				ViewBag.ExceptionMessage = ex.Message;
				return await Task.FromResult<IViewComponentResult>(View("Error"));
			}
			catch (Exception ex)
			{
				ViewBag.ExceptionMessage = ex.Message;
				return await Task.FromResult<IViewComponentResult>(View("Error"));
			}
		}
	}
}
