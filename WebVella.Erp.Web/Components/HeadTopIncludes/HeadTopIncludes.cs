﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebVella.Erp.Web.Hooks;
using WebVella.Erp.Web.Models;

namespace WebVella.Erp.Web.Components
{

	[RenderHookAttachment("head-top", 10)]
	public class HeadTopIncludes : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(BaseErpPageModel pageModel)
        {
			ViewBag.MetaTags = new List<MetaTagInclude>();
			ViewBag.LinkTags = new List<LinkTagInclude>();
			ViewBag.ScriptTags = new List<ScriptTagInclude>();

			#region == <title> ==
			var includedTitle = pageModel.HttpContext.Items.ContainsKey("<title>") ? (string)pageModel.HttpContext.Items["<title>"] : "";
			ViewBag.Title = "";
			if (string.IsNullOrWhiteSpace(includedTitle))
			{
				var titleTag = "<title>" + pageModel.PageContext.ViewData["Title"] + "</title>";
				ViewBag.Title = titleTag;
				pageModel.HttpContext.Items["<title>"] = titleTag;
			}
			#endregion

			#region === <meta> ===
			{
				var includedMetaTags = pageModel.HttpContext.Items.ContainsKey(typeof(List<MetaTagInclude>)) ? (List<MetaTagInclude>)pageModel.HttpContext.Items[typeof(List<MetaTagInclude>)] : new List<MetaTagInclude>();
				var metaTagsToInclude = new List<MetaTagInclude>();
				//Your includes below >>>>

				#region << viewport >>
				{
					var tagName = "viewpost";
					if (!includedMetaTags.Any(x => x.Name == tagName))
					{
						metaTagsToInclude.Add(new MetaTagInclude()
						{
							Name = tagName,
							Content = "width=device-width, initial-scale=1, shrink-to-fit=no"
						});
					}
				}
				#endregion

				#region << charset >>
				{
					var tagName = "charset";
					if (!includedMetaTags.Any(x => x.Name == tagName))
					{
						metaTagsToInclude.Add(new MetaTagInclude()
						{
							Charset = "utf-8"
						});
					}
				}
				#endregion

				//<<<< Your includes up
				includedMetaTags.AddRange(metaTagsToInclude);
				pageModel.HttpContext.Items[typeof(List<MetaTagInclude>)] = includedMetaTags;
				ViewBag.MetaTags = metaTagsToInclude;
			}
			#endregion


			#region === <link> ===
			{
				var includedLinkTags = pageModel.HttpContext.Items.ContainsKey(typeof(List<LinkTagInclude>)) ? (List<LinkTagInclude>)pageModel.HttpContext.Items[typeof(List<LinkTagInclude>)] : new List<LinkTagInclude>();
				var linkTagsToInclude = new List<LinkTagInclude>();
				
				//Your includes below >>>>

				#region << favicon >>
				{
					if (!includedLinkTags.Any(x => x.Href.Contains("favicon")))
					{
						linkTagsToInclude.Add(new LinkTagInclude()
						{
							Href = $"/assets/favicon.png",
							Rel = RelType.Icon,
							Type = "image/png"
						});
					}
				}
				#endregion

				#region << framework >>
				{
					//Always include
					linkTagsToInclude.Add(new LinkTagInclude()
					{
						Href = "/api/v3.0/p/core/framework.css",
						CacheBreaker = pageModel.ErpAppContext.StyleFrameworkHash,
						CrossOrigin = CrossOriginType.Anonymous,
						Integrity = $"sha256-{pageModel.ErpAppContext.StyleFrameworkHash}"
					});
				}
				#endregion

				#region << themify >>
				{
					if (!includedLinkTags.Any(x => x.Href.Contains("/themify")))
					{
						linkTagsToInclude.Add(new LinkTagInclude()
						{
							Href = "/js/themify-icons-0.1.2/css/themify-icons.css"
						});
					}
				}
				#endregion

				#region << flatpickr >>
				{
					if (!includedLinkTags.Any(x => x.Href.Contains("/flatpickr")))
					{
						linkTagsToInclude.Add(new LinkTagInclude()
						{
							Href = "/lib/flatpickr/flatpickr.min.css"
						});
					}
				}
				#endregion

				#region << select2 >>
				{
					if (!includedLinkTags.Any(x => x.Href.Contains("/select2")))
					{
						linkTagsToInclude.Add(new LinkTagInclude()
						{
							Href = "/lib/select2/css/select2.min.css"
						});
					}
				}
				#endregion

				#region << font-awesome >>
				{
					if (!includedLinkTags.Any(x => x.Href.Contains("/font-awesome")))
					{
						linkTagsToInclude.Add(new LinkTagInclude()
						{
							Href = "/js/font-awesome-5.4.1/css/all.min.css"
						});
					}
				}
				#endregion

				#region << toastr >>
				{
					if (!includedLinkTags.Any(x => x.Href.Contains("/toastr")))
					{
						linkTagsToInclude.Add(new LinkTagInclude()
						{
							Href = "/lib/toastr.js/toastr.min.css"
						});
					}
				}
				#endregion

				#region << colorpicker >>
				{
					if (!includedLinkTags.Any(x => x.Href.Contains("/colorpicker")))
					{
						linkTagsToInclude.Add(new LinkTagInclude()
						{
							Href = "/lib/spectrum/spectrum.min.css"
						});
					}
				}
				#endregion

				//<<<< Your includes up

				includedLinkTags.AddRange(linkTagsToInclude);
				pageModel.HttpContext.Items[typeof(List<LinkTagInclude>)] = includedLinkTags;
				ViewBag.LinkTags = linkTagsToInclude;
			}
			#endregion

			#region === <script> ===
			{
				var includedScriptTags = pageModel.HttpContext.Items.ContainsKey(typeof(List<ScriptTagInclude>)) ? (List<ScriptTagInclude>)pageModel.HttpContext.Items[typeof(List<ScriptTagInclude>)] : new List<ScriptTagInclude>();
				var scriptTagsToInclude = new List<ScriptTagInclude>();

				//Your includes below >>>>


				//<<<< Your includes up
				includedScriptTags.AddRange(scriptTagsToInclude);
				pageModel.HttpContext.Items[typeof(List<ScriptTagInclude>)] = includedScriptTags;
				ViewBag.ScriptTags = scriptTagsToInclude;
			}
			#endregion

			return await Task.FromResult<IViewComponentResult>(View("Default"));
        }
    }
}
