﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

using WebApp_FizzBuzz.Models;
using WebApp_FizzBuzz.Data;

namespace WebApp_FizzBuzz.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        [Display(Name = "FizzBuzz")]
        [Required(ErrorMessage = "Value has to be in range from 1 to 1000!")]
        [Range(1, 1000, ErrorMessage= "Value has to be in range from 1 to 1000!")]
        public int? input { get; set; }

        public FizzBuzzEntry lastEntry { get; private set; }

        private readonly ILogger<IndexModel> _logger;
        readonly FizzBuzzContext _context;

        public IndexModel(ILogger<IndexModel> logger, FizzBuzzContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            lastEntry = null;
            try
            {
                //input = HttpContext.Session.GetInt32("input");
                lastEntry = JsonConvert.DeserializeObject<FizzBuzzEntry>(HttpContext.Session.GetString("lastEntry"));
            }
            catch (Exception e)
            {
                // Ignore
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();
            lastEntry = new FizzBuzzEntry(input.Value);
            HttpContext.Session.SetString("lastEntry", JsonConvert.SerializeObject(lastEntry));

            // Saving to list in this Session.
            if(!(lastEntry is null))
            {
                SaveEntryToSession(lastEntry);
                SaveEntryToDatabase(lastEntry);
            }

            return Page();
        }

        void SaveEntryToSession(FizzBuzzEntry entry)
        {
            string list_serialized_ = HttpContext.Session.GetString("entriesList");
            if (list_serialized_ is null)
                list_serialized_ = JsonConvert.SerializeObject(new List<FizzBuzzEntry>());

            var list_ = JsonConvert.DeserializeObject<List<FizzBuzzEntry>>(
                list_serialized_
                );
            list_.Add(entry);
            HttpContext.Session.SetString("entriesList",
                JsonConvert.SerializeObject(list_)
                );
        }
        void SaveEntryToDatabase(FizzBuzzEntry entry)
        {
            _context.FizzBuzzEntries.Add(entry);
            _context.SaveChanges();
        }
    }
}
