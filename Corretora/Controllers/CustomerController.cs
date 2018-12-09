using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Corretora.Models;

namespace Corretora.Controllers
{
    public class CustomerController : Controller
    {
        private CorretoraContext db = new CorretoraContext();

        public ActionResult Index()
        {
            ViewBag.Accounts = (from conta in db.Accounts
                                select conta).ToList();

            var lista = (from cliente in db.Customers
                         orderby cliente.Name
                         select cliente).ToList();

            return View(lista);
        }

        public ActionResult Create()
        {
            return View();
        }
       
        public ActionResult Details(int id)
        {
            ViewBag.Orders = (from ordem in db.Orders
                              where ordem.CustomerID == id && ordem.Status != Status.Aberta
                              orderby ordem.Date descending
                              select ordem).ToList();

            ViewBag.Assets = (from ativo in db.Assets
                              select ativo).ToList();

            var info = (from cliente in db.Customers
                        where cliente.CustomerID == id
                        select cliente).FirstOrDefault();

            return View(info);
        }

        public ActionResult Save(Customer customer, int Balance)
        {
            var account = new Account();

            try
            {
                if (ModelState.IsValid)
                {
                    db.Customers.Add(customer);
                    db.SaveChanges();

                    account.Balance = decimal.Parse(Balance.ToString().Insert(Balance.ToString().Length - 2, ","));
                    account.CustomerID = customer.CustomerID;

                    db.Accounts.Add(account);
                    db.SaveChanges();
                    ViewBag.Mensagem = "Cliente cadastrado com sucesso!";
                }
                else { ViewBag.Mensagem = "Dados inválidos!"; }
            }
            catch (Exception ex)
            {
                db.Entry(customer).State = EntityState.Detached;
                db.Entry(account).State = EntityState.Detached;
                ViewBag.Mensagem = ex.Message;
            }
            return View("Create");
        }

        public ActionResult Alter(Customer customer, int Balance)
        {
            var account = (from conta in db.Accounts
                           where conta.CustomerID == customer.CustomerID
                           select conta).FirstOrDefault();
        
            try
            {
                if (ModelState.IsValid)
                {
                    db.Customers.Attach(customer);
                    db.Entry(customer).State = EntityState.Modified;
                    db.SaveChanges();

                    if (account != null)
                    {
                        account.Balance = decimal.Parse(Balance.ToString().Insert(Balance.ToString().Length - 2, ","));
                        db.Accounts.Attach(account);
                        db.Entry(account).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    ViewBag.Mensagem = "Cliente alterado com sucesso!";
                }
                else { ViewBag.Mensagem = "Dados inválidos!"; }
            }
            catch (Exception ex)
            {
                if (account != null) { db.Entry(account).State = EntityState.Detached; }
                db.Entry(customer).State = EntityState.Detached;
                ViewBag.Mensagem = ex.Message;
            }
            return View("Create");
        }

        public ActionResult Delete(int id)
        {
            var customer = (from cliente in db.Customers
                            where cliente.CustomerID == id
                            select cliente).FirstOrDefault();

            var account = (from conta in db.Accounts
                           where conta.CustomerID == id
                           select conta).FirstOrDefault();

            var orders = (from ordem in db.Orders
                          where ordem.CustomerID == id
                          select ordem).ToList();

            try
            {
                orders.ForEach((item) => { db.Orders.Attach(item); db.Entry(item).State = EntityState.Deleted; });
                db.SaveChanges();

                if (account != null)
                {
                    db.Accounts.Attach(account);
                    db.Entry(account).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                
                db.Customers.Attach(customer);
                db.Entry(customer).State = EntityState.Deleted;
                db.SaveChanges();
                ViewBag.Mensagem = "Cliente deletado!";
            }
            catch (Exception ex)
            {
                orders.ForEach((item) => { db.Entry(item).State = EntityState.Detached; });
                if (account != null) { db.Entry(account).State = EntityState.Detached; }
                db.Entry(customer).State = EntityState.Detached;
                ViewBag.Mensagem = ex.Message;
            }
            return View("Create");
        }
        
        public ActionResult GetCustomers()
        {
            var lista = (from cliente in db.Customers
                         orderby cliente.Name
                         select cliente).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }
    }
}