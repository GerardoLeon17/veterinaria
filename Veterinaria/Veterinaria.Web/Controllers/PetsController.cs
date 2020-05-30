using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Veterinaria.Web.Models;

namespace Veterinaria.Web.Controllers
{
    public class PetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Pets
        public ActionResult AllPets()
        {
            var pets = db.Pets.Include(o => o.Owner).Include(u => u.Owner.ApplicationUser).ToList();
            return View(pets);
        }
        public ActionResult Index()
        {
            var user = User.Identity.GetUserId();
            var ow = db.Owners.Where(o => o.UserId == user).FirstOrDefault();
            var pets = db.Pets.Include(u => u.Owner).Where(p => p.OwnerId == ow.Id).ToList();

            return View(pets);
        }

        // GET: Pets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pet pet = db.Pets.Find(id);
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }

        // GET: Pets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pet pet)
        {


            HttpPostedFileBase FileBase = Request.Files[0];

            if (FileBase.ContentLength == 0)
            {
                ModelState.AddModelError("ImageUrl", "Es necesario seleccionar una imagen.");

            }
            else
            {
                if (FileBase.FileName.EndsWith(".jpg"))
                {
                    WebImage image = new WebImage(FileBase.InputStream);
                    pet.ImgUrl = image.GetBytes();
                }

                else
                {
                    ModelState.AddModelError("ImageUrl", "Solo se aceptan imagenes con formato jpg.");

                }
            }

            if (ModelState.IsValid)
            {
                //if (hpb != null)
                //{
                //    var perfil = System.IO.Path.GetFileName(hpb.FileName);
                //    var direccion = "~/Content/Img/" + pet.Name + "_" + perfil;
                //    hpb.SaveAs(Server.MapPath(direccion));
                //    pet.ImgUrl = pet.Name + "_" + perfil;
                //}


                //SI esta autenticado un Cliente
                var userId = User.Identity.GetUserId();
                //Para Traer el Usuario de la bd.
                var own = db.Owners.Where(o => o.UserId == userId).FirstOrDefault();
                //Agregamos el Id del Own que buscamos
                pet.OwnerId = own.Id;
                db.Pets.Add(pet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pet);
        }

        // GET: Pets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pet pet = db.Pets.Find(id);
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }

        // POST: Pets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,PetType,Age,BirthDate,Color,Race,Weight,Height")] Pet pet)
        {
            byte[] ImagenActual = null;

            HttpPostedFileBase FileBase = Request.Files[0];
            if (FileBase.ContentLength == 0)
            {
                ImagenActual = db.Pets.SingleOrDefault(t => t.Id == pet.Id).ImgUrl;
                pet.ImgUrl = ImagenActual;

            }
            else
            {
                if (FileBase.FileName.EndsWith(".jpg"))
                {
                    WebImage image = new WebImage(FileBase.InputStream);
                    pet.ImgUrl = image.GetBytes();
                }

                else
                {
                    ModelState.AddModelError("ImageUrl", "Solo se aceptan imagenes con formato jpg.");

                }
            }
            if (ModelState.IsValid)
            {
                db.Entry(pet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pet);
        }

        // GET: Pets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pet pet = db.Pets.Find(id);
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }

        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pet pet = db.Pets.Find(id);
            db.Pets.Remove(pet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult getimagen(int id)
        {
            Pet produi = db.Pets.Find(id);
            byte[] byteImage = produi.ImgUrl;
            MemoryStream memoryStream = new MemoryStream(byteImage);
            Image image = Image.FromStream(memoryStream);
            memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;

            return File(memoryStream, "image/jpg");
        }
    }
}
