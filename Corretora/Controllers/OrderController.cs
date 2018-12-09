using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Corretora.Models;

namespace Corretora.Controllers
{
    public class OrderController : Controller
    {
        private CorretoraContext db = new CorretoraContext();

        public ActionResult Index()
        {
            ViewBag.Customers = (from cliente in db.Customers
                                 orderby cliente.Name
                                 select cliente).ToList();

            ViewBag.Assets = (from ativo in db.Assets
                              orderby ativo.Name
                              select ativo).ToList();

            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var order = (from ordem in db.Orders
                         where ordem.OrderID == id
                         select ordem).FirstOrDefault();

            ViewBag.Customer = (from customer in db.Customers
                                where customer.CustomerID == order.CustomerID
                                select customer).FirstOrDefault();

            ViewBag.Asset = (from asset in db.Assets
                             where asset.AssetID == order.AssetID
                             select asset).FirstOrDefault();

            return View(order);
        }

        public ActionResult Search(int ativo, int cliente)
        {
            ViewBag.Customers = (from customers in db.Customers
                                 orderby customers.Name
                                 select customers).ToList();

            ViewBag.Assets = (from assets in db.Assets
                              orderby assets.Name
                              select assets).ToList();

            var lista = new List<Order>();
            
            if (ativo != 0 && cliente != 0)
            {
                lista = (from ordem in db.Orders
                         where ordem.AssetID == ativo && ordem.CustomerID == cliente
                         orderby ordem.Date descending
                         select ordem).ToList();
            }
            else if(ativo != 0)
            {
                lista = (from ordem in db.Orders
                         where ordem.AssetID == ativo
                         orderby ordem.Date descending
                         select ordem).ToList();
            }
            else if(cliente != 0)
            {
                lista = (from ordem in db.Orders
                         where ordem.CustomerID == cliente
                         orderby ordem.Date descending
                         select ordem).ToList();
            }
            else
            {
                lista = (from ordem in db.Orders
                         orderby ordem.Date descending
                         select ordem).ToList();
            }
            return View("Index", lista);
        }

        public ActionResult SaveOrder(Order order)
        {
            try
            {
                if (order.Status == Status.Cancelada)
                {
                    var ordemCancel = (from ordem in db.Orders
                                       where ordem.OrderID == order.OrderID
                                       select ordem).FirstOrDefault();

                    ordemCancel.Status = Status.Cancelada;
                    ordemCancel.Date = DateTime.Now;

                    db.Orders.Attach(ordemCancel);
                    db.Entry(ordemCancel).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Mensagem = "Ordem cancelada!";
                }
                else if (ModelState.IsValid)
                {
                    if (order.OrderID != 0)
                    {
                        var orderAlter = (from ordem in db.Orders
                                          where ordem.OrderID == order.OrderID
                                          select ordem).FirstOrDefault();

                        orderAlter.Date = DateTime.Now;
                        orderAlter.OrderValue = order.OrderValue;
                        orderAlter.Quantity = order.Quantity;
                        orderAlter.Type = order.Type;

                        db.Orders.Attach(orderAlter);
                        db.Entry(orderAlter).State = EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.Mensagem = "Ordem alterada!";
                    }
                    else
                    {
                        order.Date = DateTime.Now;
                        order.Status = Status.Aberta;

                        db.Orders.Add(order);
                        db.SaveChanges();
                        ViewBag.Mensagem = "Ordem criada com sucesso!";
                    }
                }
                else { ViewBag.Mensagem = "Dados incompletos!"; }
            }
            catch (Exception ex)
            {
                db.Entry(order).State = EntityState.Detached;
                ViewBag.Mensagem = ex.Message;
            }
            return View("Create");
        }

        public ActionResult Execute(int id)
        {
            var ordem = (from order in db.Orders
                         where order.OrderID == id
                         select order).FirstOrDefault();

            ViewBag.Customer = (from customer in db.Customers
                                where customer.CustomerID == ordem.CustomerID
                                select customer).FirstOrDefault();

            ViewBag.Asset = (from asset in db.Assets
                             where asset.AssetID == ordem.AssetID
                             select asset).FirstOrDefault();

            return View(ordem);
        }
        public ActionResult ExecuteOrder(int OrderId, int ClienteID)
        {
            if (ClienteID != 0)
            {
                var order = (from ordem in db.Orders
                             where ordem.OrderID == OrderId
                             select ordem).FirstOrDefault();

                var contaCliente = (from conta in db.Accounts
                                    where conta.CustomerID == order.CustomerID
                                    select conta).FirstOrDefault();

                var contaExecutor = (from conta in db.Accounts
                                     where conta.CustomerID == ClienteID
                                     select conta).FirstOrDefault();

                var newOrder = new Order();
                newOrder.AssetID = order.AssetID;
                newOrder.CustomerID = contaExecutor.CustomerID;
                newOrder.OrderValue = order.OrderValue;
                newOrder.Quantity = order.Quantity;
                newOrder.Status = Status.Executada;

                if (order.Type == Models.Type.Compra)
                {
                    if (contaCliente.Balance >= order.OrderValue)
                    {
                        contaCliente.Balance -= order.OrderValue;
                        contaExecutor.Balance += order.OrderValue;

                        order.Date = DateTime.Now;
                        order.Status = Status.Executada;
                        
                        newOrder.Date = DateTime.Now;
                        newOrder.Type = Models.Type.Venda;

                        db.Orders.Add(newOrder);
                        db.SaveChanges();

                        ViewBag.Mensagem = "Ordem executada com sucesso!";
                    }
                    else { ViewBag.Mensagem = "Saldo insulficiente do cliente!"; }
                }
                else
                {
                    if (contaExecutor.Balance >= order.OrderValue)
                    {
                        contaCliente.Balance += order.OrderValue;
                        contaExecutor.Balance -= order.OrderValue;

                        order.Date = DateTime.Now;
                        order.Status = Status.Executada;

                        newOrder.Date = DateTime.Now;
                        newOrder.Type = Models.Type.Compra;

                        db.Orders.Add(newOrder);
                        db.SaveChanges();

                        ViewBag.Mensagem = "Ordem executada com sucesso!";
                    }
                    else { ViewBag.Mensagem = "Saldo insulficiente do Executor!"; }
                }
            }
            else { ViewBag.Mensagem = "Dados invalidos!"; }

            ViewBag.Customers = (from cliente in db.Customers
                                 orderby cliente.Name
                                 select cliente).ToList();

            ViewBag.Assets = (from ativo in db.Assets
                              orderby ativo.Name
                              select ativo).ToList();

            return View("Index");
        }
    }
}