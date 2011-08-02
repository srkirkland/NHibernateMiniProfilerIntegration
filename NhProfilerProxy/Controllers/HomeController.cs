using System;
using System.Web.Mvc;
using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;
using NhProfilerProxy.Helpers;

namespace NhProfilerProxy.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            var configuration = Fluently
                .Configure()
                .Database(
                    FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2008
                        .AdoNetBatchSize(25) //Commenting out the batching removes the error (though of course is not desired)
                        .ConnectionString(x => x.Server(@".\SQLExpress").Database("Northwind").TrustedConnection())
                        .Driver<ProfiledSql2008ClientDriver>() //Profiled sql client driver
                )
                .Mappings(x => x.FluentMappings.AddFromAssemblyOf<Order>());

            var sessionFactory = configuration.BuildSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var order = session.Get<Order>(11086);

                    //Getting the order doesn't thrown an error, but when a change is made and saved, the error is thrown
                    order.ShipName = "Shipper #" + new Random().Next(1000);
                   
                    session.SaveOrUpdate(order);

                    transaction.Commit();
                }
            }

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }

    public class Order
    {
        public virtual int OrderId { get; set; }
        public virtual string CustomerId { get; set; }
        public virtual string ShipName { get; set; }
    }

    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Table("Orders");

            Id(x => x.OrderId);

            Map(x => x.CustomerId);
            Map(x => x.ShipName);
        }
    }
}
