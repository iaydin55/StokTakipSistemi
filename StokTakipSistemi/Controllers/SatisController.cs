using StokTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
namespace StokTakipSistemi.Controllers
{
    public class SatisController : Controller
    {
        StokTakipSistemiEntities db = new StokTakipSistemiEntities();
        // GET: Satis
        public ActionResult Index(int sayfa = 1)
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var kullanici = db.Kullanici.Where(x => x.Email == kullaniciadi).FirstOrDefault();
                var model = db.Satislar.Where(x => x.KullaniciId == kullanici.Id).ToList().ToPagedList(sayfa, 5);
                return View(model);
            }
            return HttpNotFound();

        }

        public ActionResult SatinAl(int id)
        {
            var model = db.Sepet.FirstOrDefault(x => x.Id == id);
            return View(model);
        }
        [HttpPost]
        public ActionResult SatinAl2(int id)
        {
            try
            {
                if ((ModelState.IsValid))
                {
                    var model = db.Sepet.FirstOrDefault(x => x.Id == id);
                    var satis = new Satislar
                    {
                        KullaniciId = model.KullaniciId,
                        UrunId = model.UrunId,
                        Adet = model.Adet,
                        Resim = model.Resim,
                        Tarih = DateTime.Now,
                        Fiyat = model.Fiyat
                    };
                    db.Sepet.Remove(model);
                    db.Satislar.Add(satis);
                    db.SaveChanges();
                    ViewBag.sonuc = "Satın alma işlemi başarılı";
                }
            }
            catch (Exception)
            {
                ViewBag.sonuc = "Satın alma işlemi başarısız";
            }
            return View("islem");
        }

        public ActionResult HepsiniSatinAl(decimal? Tutar)
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
                    else if (kid != null)
                    {
                        Tutar = db.Sepet.Where(x => x.KullaniciId == kullanici.Id).Sum(x => x.Urun.Fiyat * x.Adet);
                        ViewBag.Tutar = "Toplam tutar = " + Tutar + "TL";
                    }
                    return View(model);
                }

            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult HepsiniSatinAl2()
        {
            var kullaniciadi = User.Identity.Name;
            var kullanici = db.Kullanici.Where(x => x.Email == kullaniciadi).FirstOrDefault();
            var model = db.Sepet.Where(x => x.KullaniciId == kullanici.Id).ToList();
            int satir = 0;
            foreach (var item in model)
            {
                var satis = new Satislar
                {
                    KullaniciId = item.KullaniciId,
                    UrunId = item.UrunId,
                    Adet = item.Adet,
                    Resim = item.Resim,
                    Tarih = DateTime.Now,
                    Fiyat = item.Fiyat
                };
                db.Satislar.Add(satis);
                db.SaveChanges();
                satir++;
            }
            db.Sepet.RemoveRange(model);
            db.SaveChanges();
            return RedirectToAction("Index","Sepet");
        }
    }
}