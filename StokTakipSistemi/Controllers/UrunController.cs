using StokTakipSistemi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace StokTakipSistemi.Controllers
{
    public class UrunController : Controller
    {
        StokTakipSistemiEntities db = new StokTakipSistemiEntities();

        // GET: Urun
        [Authorize]
        public ActionResult Index()
        {
            var list = db.Urun.ToList();
            return View(list);
        }
        [Authorize(Roles = "A")]
        [HttpGet]
        public ActionResult UrunEkle()
        {
            List<SelectListItem> deger1 = (from x in db.Kategori.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.Ad,
                                               Value = x.Id.ToString()
                                           }).ToList();
            ViewBag.ktgr= deger1;
            return View();
        }
        [Authorize(Roles = "A")]
        [HttpPost]
        public ActionResult UrunEkle(Urun Data, HttpPostedFileBase File)
        {
            string path = Path.Combine("~/Content/Images/", File.FileName);
            File.SaveAs(Server.MapPath(path));
            Data.Resim = File.FileName.ToString();
            db.Urun.Add(Data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "A")]
        public ActionResult Sil(int id)
        {
            var urun = db.Urun.Where(x=>x.Id == id).FirstOrDefault();
            db.Urun.Remove(urun);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "A")]
        public ActionResult Guncelle(int id)
        {
            var guncelle = db.Urun.Where(x => x.Id == id).FirstOrDefault();
            List<SelectListItem> deger1 = (from x in db.Kategori.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.Ad,
                                               Value = x.Id.ToString()
                                           }).ToList();
            ViewBag.ktgr = deger1;
            return View(guncelle);
        }
        [Authorize(Roles = "A")]
        [HttpPost]
        public ActionResult Guncelle(Urun Data, HttpPostedFileBase File)
        {
            var guncelle = db.Urun.Find(Data.Id);
            //if (File != null)
            //{
            //    string path = Path.Combine("~/Content/Images/", File.FileName);
            //    File.SaveAs(Server.MapPath(path));
            //    guncelle.Resim = File.FileName.ToString();
            //}
            if(File == null)
            {
                guncelle.Ad = Data.Ad;
                guncelle.Aciklama = Data.Aciklama;
                guncelle.Fiyat = Data.Fiyat;
                guncelle.Stok = Data.Stok;
                guncelle.Populer = Data.Populer;
                guncelle.KategoriId = Data.KategoriId;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            else
            {
                guncelle.Resim = File.FileName.ToString();
                guncelle.Ad = Data.Ad;
                guncelle.Aciklama = Data.Aciklama;
                guncelle.Fiyat = Data.Fiyat;
                guncelle.Stok = Data.Stok;
                guncelle.Populer = Data.Populer;
                guncelle.KategoriId = Data.KategoriId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        
        }
        [Authorize(Roles = "A")]
        public ActionResult KritikStok()
        {
            var kritik = db.Urun.Where(x => x.Stok < 50).ToList();
            return View(kritik);
        }
        public PartialViewResult StokCount()
        {
            if (User.Identity.IsAuthenticated)
            {
                var count =db.Urun.Where(x=> x.Stok < 50).Count();
                ViewBag.count = count;
               
            }
            return PartialView();
        }
        public ActionResult StokGrafik()
        {
            ArrayList deger1 = new ArrayList();
            ArrayList deger2 = new ArrayList();
            var veriler = db.Urun.ToList();
            veriler.ToList().ForEach(x => deger1.Add(x.Ad));
            veriler.ToList().ForEach(x => deger2.Add(x.Stok));
            var grafik = new Chart(width:500, height:500).AddTitle("Ürün-Stok-Grafiği").AddSeries(chartType:"Column",name:"Ad",xValue:deger1,yValues:deger2);
            
            return File(grafik.ToWebImage().GetBytes(),"image/jpeg");
        }

    }
}