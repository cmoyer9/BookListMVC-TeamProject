using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookListMVC.Controllers
{
    public class AuthorsController : Controller
    {
       
            private readonly ApplicationDBContext _db;
            [BindProperty]
            public Author Author { get; set; }
            public AuthorsController(ApplicationDBContext db)
            {
                _db = db;
            }

            public IActionResult Index()
            {
                return View();
            }

            public IActionResult Upsert(int? id)
            {
                Author = new Author();
                if (id == null)
                {
                    //create
                    return View(Author);
                }
                //update
               Author = _db.Authors.FirstOrDefault(u => u.Id == id);
                if (Author == null)
                {
                    return NotFound();
                }
                return View(Author);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Upsert()
            {
                if (ModelState.IsValid)
                {
                    if (Author.Id == 0)
                    {
                    //create
                    _db.Authors.Add(Author);
                    }
                    else
                    {
                    _db.Authors.Update(Author);
                    }
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(Author);
            }

            #region API Calls
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                return Json(new { data = await _db.Authors.ToListAsync() });
            }

            [HttpDelete]
            public async Task<IActionResult> Delete(int id)
            {
                var authorFromDb = await _db.Authors.FirstOrDefaultAsync(u => u.Id == id);
                if (authorFromDb == null)
                {
                    return Json(new { success = false, message = "Error while Deleting" });
                }
                _db.Authors.Remove(authorFromDb);
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Delete successful" });
            }
            #endregion
        }
    }