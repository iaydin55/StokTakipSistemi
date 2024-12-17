using StokTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StokTakipSistemi.Controllers
{

    public class SepetController : Controller
    {
        StokTakipSistemiEntities db = new StokTakipSistemiEntities();
        // GET: Sepet
        public ActionResult Index(decimal? Tutar)
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var kullanici = db.Kullanici.Where(x => x.Email == kullaniciadi).FirstOrDefault();
                var model = db.Sepet.Where(x => x.KullaniciId == kullanici.Id).ToList();
                var kid = db.Sepet.FirstOrDefault(Sepet => Sepet.KullaniciId == kullanici.Id);
                if (model != null)
                {
                    if (kid == null)
                    {
                        ViewBag.Tutar = "Sepetinizde Ürün Bulunmamaktadır";
                    }
                    else if(kid != null)
                    {
                        Tutar =db.Sepet.Where(x => x.KullaniciId == kullanici.Id).Sum(x => x.Urun.Fiyat * x.Adet);
                        ViewBag.Tutar = "Toplam tutar = " + Tutar+ "TL";
                    }
                    return View(model);
                }
               
            }
            return HttpNotFound();

        }
        public ActionResult SepeteEkle(int id)
        {
            if(User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var model = db.Kullanici.Where(x => x.Email == kullaniciadi).FirstOrDefault();
                var u=db.Urun.Find(id);
                var sepet = db.Sepet.Where(x => x.KullaniciId == model.Id && x.UrunId == id).FirstOrDefault();
                if (model != null)
                {
                    if(sepet != null)
                    {
                        sepet.Adet++;
                        sepet.Fiyat = u.Fiyat * sepet.Adet;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    var s = new Sepet
                    {
                        KullaniciId = model.Id,
                        UrunId = u.Id,
                        Adet = 1,
                        Fiyat = u.Fiyat,
                        Tarih = DateTime.Now
                    };
                    db.Sepet.Add(s);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View();
            }
            return HttpNotFound();
        }

        public ActionResult SepetCount(int?count)
        {
            if(User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var kullanici = db.Kullanici.Where(x => x.Email == kullaniciadi).FirstOrDefault();
                count = db.Sepet.Where(x => x.KullaniciId == kullanici.Id).Count();
                ViewBag.count = count;
                if(count == 0)
                {
                    ViewBag.count = "";
                }
                return PartialView();
            }
            return HttpNotFound();
        }

        public ActionResult arttir(int id)
        {
            var model = db.Sepet.Find(id);
            model.Adet++;
            model.Fiyat = model.Fiyat * model.Adet;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult azalt(int id)
        {
            var model = db.Sepet.Find(id);
            if(model.Adet==1)
            {
                db.Sepet.Remove(model);
                db.SaveChanges();
           
            }
            model.Adet--;
            model.Fiyat = model.Fiyat * model.Adet;

            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public void AdetYaz(int id, int miktari)
        {
            var model = db.Sepet.Find(id);
       
            model.Adet = miktari;
            model.Fiyat = model.Fiyat * model.Adet;
            db.SaveChanges();
        }


        public ActionResult Sil(int id)
        {
            var model = db.Sepet.Find(id);
            db.Sepet.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult HepsiniSil()
        {
          if(User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var model = db.Kullanici.Where(x => x.Email == kullaniciadi).FirstOrDefault();
                var sil = db.Sepet.Where(x => x.KullaniciId == model.Id).ToList();
                db.Sepet.RemoveRange(sil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return HttpNotFound();
        }
    }
}