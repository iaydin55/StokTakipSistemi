using StokTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StokTakipSistemi.Controllers
{
    [Authorize(Roles = "A")]
    public class KategoriController : Controller
    {
        StokTakipSistemiEntities db = new StokTakipSistemiEntities();
        // GET: Kategori
       
        public ActionResult Index()
        {
            return View(db.Kategori.Where(x=>x.Durum == true).ToList());
        }
     
        public ActionResult Ekle()
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult Ekle(Kategori data)
        {
            db.Kategori.Add(data);
            data.Durum = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

     
        public ActionResult Sil(int id)
        {
            var kategori = db.Kategori.Where(x => x.Id == id).FirstOrDefault();
            kategori.Durum = false;
            db.Kategori.Remove(kategori);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Guncelle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Guncelle(Kategori data)
        {
            var guncelle = db.Kategori.Where(x => x.Id == data.Id).FirstOrDefault();
            guncelle.Aciklama = data.Aciklama;
            guncelle.Ad = data.Ad;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}