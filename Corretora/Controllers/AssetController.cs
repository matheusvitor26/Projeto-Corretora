using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Corretora.Models;

namespace Corretora.Controllers
{
    public class AssetController : Controller
    {
        private CorretoraContext db = new CorretoraContext();

        public ActionResult Index()
        {
            var lista = (from ativo in db.Assets
                         orderby ativo.Name
                         select ativo).ToList();

            return View(lista);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Save(Asset asset)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var exist = (from ativo in db.Assets
                                 where ativo.Name == asset.Name
                                 select ativo).FirstOrDefault();

                    if (exist == null)
                    {
                        asset.Name = asset.Name.ToUpper();
                        asset.UnitPrice = decimal.Parse(asset.UnitPrice.ToString().Insert(asset.UnitPrice.ToString().Length - 2, ","));

                        db.Assets.Add(asset);
                        db.SaveChanges();
                        ViewBag.Mensagem = "Ativo cadastrado com sucesso!";
                    }
                    else { ViewBag.Mensagem = "Ativo ja existe!"; }
                }
                else { ViewBag.Message = "Dados incompletos!"; }
            }
            catch (Exception ex)
            {
                db.Entry(asset).State = EntityState.Detached;
                ViewBag.Mensagem = ex.Message;
            }
            return View("Create");
        }
        
        public ActionResult Delete(int id)
        {
            var ativo = db.Assets.SingleOrDefault(a => a.AssetID == id);
            var ordens = db.Orders.Where(o => o.AssetID == id).ToList();

            try
            {
                ordens.ForEach((item) => { db.Orders.Attach(item); db.Entry(item).State = EntityState.Deleted; });
                db.SaveChanges();
                db.Assets.Attach(ativo);
                db.Entry(ativo).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception)
            {
                ordens.ForEach((item) => { db.Entry(item).State = EntityState.Detached; });
                db.Entry(ativo).State = EntityState.Detached;
            }
            return RedirectToAction("Index");
        }

        public ActionResult GetAtivos()
        {
            var lista = (from ativo in db.Assets
                         orderby ativo.Name
                         select ativo).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPrice(int id)
        {
            var valor = (from ativo in db.Assets
                         where ativo.AssetID == id
                         select ativo.UnitPrice);

            return Json(valor, JsonRequestBehavior.AllowGet);
        }
    }
}