﻿using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using ScrapeResults.Models;
using ScrapeResults.ViewModels;
using System.Diagnostics;
using System.Net.Http;
using System.Xml.Linq;

namespace ScrapeResults.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}