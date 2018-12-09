using System.Linq;
using System.Web.Mvc;
using Corretora.Models;

namespace Corretora.Controllers
{
    public class AccountController : Controller
    {
        private CorretoraContext db = new CorretoraContext();

        public ActionResult GetBalance(int id)
        {
            var lista = (from conta in db.Accounts
                         where conta.CustomerID == id
                         select conta.Balance);

            return Json(lista, JsonRequestBehavior.AllowGet);
        }
    }
}