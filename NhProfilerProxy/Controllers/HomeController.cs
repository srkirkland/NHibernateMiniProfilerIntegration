using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

            return View();
        }

        public ActionResult About()
        {
            var configuration = Fluently
                .Configure()
                .Database(
                    FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2008.AdoNetBatchSize(25).ConnectionString(
                        x => x.Server(@".\SQLExpress").Database("Northwind").TrustedConnection()).Driver
                        <ProfiledSql2008ClientDriver>())
                .Mappings(x => x.FluentMappings.AddFromAssemblyOf<Order>());

            var sessionFactory = configuration.BuildSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var order = session.Get<Order>(10249);

                    transaction.Commit();
                }
            }

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
