using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using BlogApp.Data.Migrations;
using Microsoft.Extensions.Primitives;

namespace BlogApp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AdminController(ApplicationDbContext context,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
           
        }

        // GET: Admin
      
        public async Task<IActionResult> Index()
        {
            return View(await _context.Posts.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }



            return View(post);

#pragma warning disable CS8603 // Possible null reference return.
            //return await _context.Posts.Where(x => x.Id == id).Select(post => new Post()
            //{
            //    Id = post.Id,
            //    Body = post.Body,
            //    Created = post.Created,
            //    ImagePath = post.ImagePath,
            //    Gallery = post.Gallery.Select(g => new Gallery()
            //    {
            //        Id = g.Id,
            //        Name = g.Name,
            //        URL = g.URL


            //    }).ToList()
            //}).FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,Image,ImagePath,GalleryFiles,Created")] Post post )
        {
            ModelState.Clear();
            
            if (ModelState.IsValid)
            {
               
                if (post.Image != null )
                {
                    string folder = "uploads/blog/";

                    post.ImagePath = await UploadImage(folder,post.Image);




                }

                if (post.GalleryFiles != null)
                {
                    string folder = "uploads/blogGallery/";

                    post.Gallery = new List<Gallery>();


                    foreach (var file in post.GalleryFiles)
                    {
                        var galllery = new Gallery()
                        {
                            Name = file.Name,
                            URL = await UploadImage(folder, file)
                        };
                        post.Gallery.Add(galllery);

                    }


                }
                _context.Add(post);
                await _context.SaveChangesAsync();



                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }


        private async Task<string> UploadImage(string folderPath,IFormFile file)
        {
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;


            string serverFolder = Path.Combine(this.webHostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }


        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,Image,ImagePath,GalleryFiles,Created")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            ModelState.Clear();

            if (ModelState.IsValid)
            {

                if (post.Image != null)
                {
                    string folder = "uploads/blog/";

                    post.ImagePath = await UploadImage(folder, post.Image);




                }

                if (post.GalleryFiles != null)
                {
                    string folder = "uploads/blogGallery/";

                    post.Gallery = new List<Gallery>();


                    foreach (var file in post.GalleryFiles)
                    {
                        var galllery = new Gallery()
                        {
                            Name = file.Name,
                            URL = await UploadImage(folder, file)
                        };
                        post.Gallery.Add(galllery);

                    }


                }


                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
