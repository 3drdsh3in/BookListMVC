using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDBContext _db;
        [BindProperty]
        public Book Book { get; set; }
        public BooksController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Book = new Book();
            if (id == null)
            {
                // Create
                return View(Book);
            }
            // Update (if id != null)
            Book = _db.Books.FirstOrDefault(u=>u.Id ==id);
            if (Book == null)
            {
                // No book in db
                return NotFound();
            }
            // Return Found Book from DB
            return View(Book);
        }
        [HttpPost]
        // ValidateAntiForgeryToken uses inbuilt security to prevent some cyber attacks
        [ValidateAntiForgeryToken]
        // Don't need to write this as:
        // public IActionResult Upsert(Book book) since the Book property has a [BindProperty] set above
        // (That is we can access it within Upsert() method without it beings passed as a parameter)
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Book.Id == 0)
                {
                    // create
                    _db.Books.Add(Book);
                } else
                {
                    _db.Books.Update(Book);
                }
                _db.SaveChanges();
                // Redirects to the Index action/method above that does a return View()
                return RedirectToAction("Index");
            }
            return View(Book);
        }

        #region API Calls


        // Get Route/Handler
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Books.ToListAsync() });
        }

        // Delete Route Handler
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _db.Books.FirstOrDefaultAsync(u => u.Id == id);
            if (bookFromDb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _db.Books.Remove(bookFromDb);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful" });
        }
    }
    #endregion
}
