using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using api_app.Models;
using api_app.Services;

namespace api_app.Controllers
{
    public class ViewModel
    {
        public IEnumerable<Item> Items { get; set; }
        public string InputValue { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IQueueService _queueService;
        public HomeController(IItemService itemService, IQueueService queueService)
        {
            _itemService = itemService;
            _queueService = queueService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Item> items = await _itemService.GetAsync("SELECT * FROM c");
            ViewModel model = new ViewModel();
            model.Items = items;
            model.InputValue = "1";
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ViewModel model)
        {
            Item item = new Item();
            item.Id = System.Guid.NewGuid().ToString();
            item.SessionId = "todo";
            item.Input = model.InputValue;
            item.Output = "";
            item.Completed = false;
            _itemService.AddAsync(item).Wait();

            TaskModel task = new TaskModel();
            task.Id = item.Id;
            task.Input = item.Input;
            _queueService.SendMessage(task).Wait();

            return Redirect("/");
        }
    }
}
