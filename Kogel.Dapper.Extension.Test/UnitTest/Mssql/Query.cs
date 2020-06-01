using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Digiwin.MES.Server.Application.Domain.EQP.Entities;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Model;
using Kogel.Dapper.Extension.MsSql;
using Kogel.Dapper.Extension.MsSql.Extension;
using Kogel.Dapper.Extension.Test.Model;
using Lige.Model;
using Lige.ViewModel.APP.Shopping;
using Lige.ViewModel.Web;
using Lige.ViewModel.Web.AppUser;

namespace Kogel.Dapper.Extension.Test.UnitTest.Mssql
{
    public class Query
    {
        string mssqlConnection = "Data Source=localhost;Initial Catalog=Qx_Sport_Common;User ID=qxdev;Password=qxdev123456;";
        public void Test()
        {
            //        using (var conn = new SqlConnection(mssqlConnection))
            //        {
            //var aaa = conn.QuerySet<Comment>().Where(x => 1 == 1).ToList();

            ////单个属性返回
            //var ContentList = conn.QuerySet<Comment>()
            //                 .AsTableName(typeof(Comment), "Comment_4")
            //                 .Where(x => x.Id > 0)
            //                 .ToList(x => x.Content);
            //            //单条记录
            //            var commne = conn.QuerySet<Comment>()
            //                .AsTableName(typeof(Comment), "Comment_4")
            //                .Where(x => x.Id > 0)
            //                .Get();

            //            //翻页
            //            var comment1 = conn.QuerySet<Comment>()
            //                .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
            //                .Where(x => x.Id.Between(80, 100)
            //                && x.SubTime.AddDays(-10) < DateTime.Now && x.Id > 10
            //                && x.Id > new QuerySet<News>(conn, new MsSqlProvider()).Where(y => y.Id < 3 && x.Id < y.Id).Sum<News>(y => y.Id))
            //                .From<Comment, News>()
            //                .OrderBy<News>(x => x.Id)
            //                .PageList(1, 1, (a, b) => new
            //                {
            //                    test = new List<int>() { 3, 3, 1 }.FirstOrDefault(y => y == 1),
            //                    aaa = "6666" + "777",
            //                    Content = a.Content + "'test'" + b.Headlines + a.IdentityId,
            //                    bbb = new QuerySet<Comment>(conn, new MsSqlProvider())
            //                                .Where(y => y.ArticleId == b.Id && y.Content.Contains("test"))
            //                                .Sum<Comment>(x => x.Id),
            //                    ccc = a.IdentityId,
            //                    ddd = Convert.ToInt32("(select count(1) from Comment)"),
            //                    a.Id,
            //                    cccTime = DateTime.Now
            //                });

            //            //计总
            //            var sum = conn.QuerySet<Comment>()
            //                   .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
            //                   .Where(x => x.Content == "test")
            //                   .WithNoLock()
            //                   .Sum<News>(x => x.Id);
            //            //计数
            //            var count = conn.QuerySet<Comment>()
            //                 .Join<Comment, News>((a, b) => a.ArticleId == b.Id)
            //                 .Where(x => x.Content == "test1")
            //                 .Where<News>(x => x.NewsLabel.Contains("足球"))
            //                 .WithNoLock()
            //                 .Count();
            //        }
            SqlMapper.Aop.OnExecuting += Aop_OnExecuting;

            using (var connection = new SqlConnection("server=localhost;database=Lige;user=sa;password=!RisingupTech/././.;"))
            {
                EntityCache.Register(typeof(EQP_TYPE_BAS));

                ////测试codefirst
                //CodeFirst codeFirst = new CodeFirst(connection);
                //codeFirst.SyncStructure();


                //SqlMapper.RemoveTypeMap(typeof(EQP_TYPE_BAS));
                //var result = connection.QuerySet<EQP_TYPE_BAS>()
                //.Where(x => x.DELETE_FLAG == "N")
                //.Where( x => x.EQP_TYPE_NO.Contains("") || x.EQP_TYPE_NAME.Contains(""))
                //.Distinct()
                //.OrderBy(x => x.CREATE_TIME)
                //.PageList(1, 10);


                var test = connection.QuerySet<Product>()
                    .Where(x => x.Price.IsNull())
                    .Get(x => new
                    {
                        x.Id,
                        x.Price
                    });


                var pageList2 = connection.QuerySet<Advert>()
             //.Where(pageListReqDto.dynamicWhere)
             .Where(x => x.IsDelete == false)
             .Distinct()
             .Join<Advert, Store>((a, b) => a.AssoId == b.Id, JoinMode.LEFT)
             .Where<Advert, Store>((a, b) => a.Id != 1 && b.Id != 2)
             .From<Advert, Store>()
             .OrderByDescing<Advert>(x => x.Seq)
             .PageList(1, 10, (x, y) => new AdvertListDto()
             {
                 Id = x.Id,
                 Title_CN = x.Title_CN,
                 Title_EN = x.Title_EN,
                 ImgUrl_CN = "123" + x.ImgUrl_CN,
                 ImgUrl_EN = GlobalConfig.ResourcesUrl + x.ImgUrl_EN,
                 Content_CN = x.Content_CN,
                 Content_EN = x.Content_EN,
                 Type = x.Type,
                 AssoId = x.AssoId,
                 AssoName = y.Name_CN,
                 IsOnline = x.IsOnline,
                 Sort = x.Sort,
                 MarketId = x.MarketId,
                 Seq = x.Seq,
                 DetailImgUrl_CN = GlobalConfig.ResourcesUrl + x.DetailImgUrl_CN,
                 DetailImgUrl_EN = GlobalConfig.ResourcesUrl + x.DetailImgUrl_EN,
                 StartTime = x.StartTime,
                 EndTime = x.EndTime
             });




                string account = "admin";
                string password = "123456";

                var adminUser = connection.QuerySet<AdminUser>()
                    .Where(x => x.Account == account && x.Password == password)
                    .Where(x => x.IsDelete == false && x.IsDelete != x.IsDelete)
                    .Get(x => new AdminUser
                    {
                        Account = x.Password
                    });

                var outStockList = new List<string>()
                {
                    "6FACCBB4-C378-4CE0-8BAB-37D16B612426",
"6FACCBB4-C378-4CE0-8BAB-37D16B612426",
"C7F4300A-5166-4177-A45A-8E77027D6668",
"97206B94-EEC1-443A-8B37-D359D5206986",
"19D1D6F9-E311-490B-A692-1B2858C0D9B0",
"1DBA06C6-DFCA-484C-ABF4-293D7A43ED11",
"ED761DEA-C57D-4B9C-8000-E434FE280337",
                }.ToArray();
                //把有库存的恢复
                connection.CommandSet<Product>()
                    .Where(x => outStockList.Contains(x.ProductCode))
                    .Update(x => new Product()
                    {
                        IsDelete = false,
                        UpdateUser = "admin sync",
                        UpdateDate = DateTime.Now
                    });

                var pageList = connection.QuerySet<Lige.Model.Order>()
                    .WhereIf(0 != 0, x => x.IsDelete == false && x.Status == 0, x => x.IsDelete == false)
                    .OrderByDescing(x => x.CreateDate)
                    .PageList(1, 10, x => new OrderResDto()
                    {
                        Id = x.Id,
                        OrderNo = x.OrderNo,
                        OrderTime = x.CreateDate,
                        Status = x.Status,
                        Amount = x.Amount,
                        Point = x.Point,
                        IsAnyOrderDetail = Convert.ToBoolean(connection.QuerySet<OrderDetail>().Where(y => y.OrderNo == x.OrderNo).Count()),
                        OrderDetailList = connection.QuerySet<OrderDetail>()
                        .Where(y => y.IsDelete == false && y.OrderNo == x.OrderNo)
                        .WhereIf(1 == 1, y => y.IsDelete == false && y.OrderNo == x.OrderNo, y => y.IsDelete == false)
                        .Join<OrderDetail, Product>((a, b) => a.ProductCode == b.ProductCode, JoinMode.LEFT, true)
                        .From<OrderDetail, Product>()
                        .OrderBy<Product>(y => y.Id)
                        .ToList((a, b) => new OrderDetailResDto()
                        {
                            Id = a.Id,
                            Name = a.ProductName,
                            Point = a.Point,
                            Price = a.Price,
                            Qty = a.Qty,
                            OriginalPrice = b.Price,
                            OriginalPoint = b.Point,
                        }),
                        DetailList = connection.QuerySet<OrderDetail>().Where(y => y.OrderNo == x.OrderNo).Get(),
                    });
                //List<string> N5_ProfileIdList = new List<string>() { "510002443", "510002444", "510002445", "510002446", "510002447", "510002449", "510002458" };
                //var list = connection.QuerySet<PurchaseTransaction>()
                // .Where(x => x.N5_ProfileId.In(N5_ProfileIdList.ToArray()) && x.TransactionAmount >= 30 && x.SqlId > 0
                // && x.CreateDateTime >= Convert.ToDateTime("2019-11-28 00:33:33.627") && !(x.InvoiceNumber.Contains("LK")))
                // .OrderBy(x => x.SqlId)
                // .ToList(x => new PurchaseTransaction()
                // {
                //	 SqlId = x.SqlId,
                //	 PhysicalCardId = x.PhysicalCardId,
                //	 TransactionAmount = x.TransactionAmount,
                //	 MachineId = x.MachineId,
                //	 N5_ProfileId = x.N5_ProfileId,
                //	 Point = Convert.ToInt32(x.TransactionAmount / 30),
                //	 InvoiceNumber = x.InvoiceNumber
                // });


                var statusArr = new int[] { 0, 2 };
                var orderList = connection.QuerySet<Order>()
                    .Where(x => statusArr.Contains(x.Status) && x.CreateDate.AddMinutes(15) < DateTime.Now && !string.IsNullOrEmpty(x.OrderNo))
                    .ToDataSet(x => new
                    {
                        x.Id,
                        x.OrderNo
                    });

                var pageLists = connection.QuerySet<Order>()
                   .OrderByDescing(x => x.CreateDate)
                   .PageList(1, 10, x => new OrderResDto()
                   {
                       Id = x.Id,
                       OrderNo = x.OrderNo,
                       OrderTime = x.CreateDate,
                       Status = x.Status,
                       Amount = x.Amount,
                       Point = x.Point,
                       DetailList = connection.QuerySet<OrderDetail>().Where(y => y.OrderNo == x.OrderNo).Get(),
                       //IsAnyOrderDetail = Convert.ToBoolean(connection.QuerySet<OrderDetail>().Where(y => y.OrderNo == x.OrderNo).Count()),
                       //OrderDetailList = connection.QuerySet<OrderDetail>()
                       //.Where(y => y.IsDelete == false && y.OrderNo == x.OrderNo)
                       //.Join<OrderDetail, Product>((a, b) => a.ProductCode == b.ProductCode, JoinMode.LEFT, true)
                       //.Join<Product, GiftDetail>((a, b) => a.ProductCode == b.ProductCode, JoinMode.LEFT, true)
                       //.From<OrderDetail, Product, GiftDetail>()
                       //.ToList(1 == 1
                       //, (a, b, c) => new OrderDetailResDto()
                       //{
                       // Id = a.Id,
                       // Name = a.ProductName,
                       // Point = a.Point,
                       // Price = a.Price,
                       // Qty = a.Qty,
                       // OriginalPrice = b.Price,
                       // OriginalPoint = b.Point,
                       // ImgUrl = c.ImgUrl_CN
                       //}
                       //, (a, b, c) => new OrderDetailResDto()
                       //{
                       // Id = a.Id,
                       // Name = a.ProductName,
                       // Point = a.Point,
                       // Price = a.Price,
                       // Qty = a.Qty,
                       // OriginalPrice = b.Price,
                       // OriginalPoint = b.Point,
                       // ImgUrl = c.ImgUrl_EN
                       //})
                   });
                ////	var test = connection.QuerySet<Order>()
                ////		.OrderBy(x => x.Id)
                ////		.PageList(1, 10);

                //////获取上次同步成功的最大id
                //var maxId = connection.QuerySet<Lige.Model.ActivitySendPoints>()
                //	.OrderByDescing(x => x.SqlId)
                //	.Get(x => x.SqlId);
                connection.CommandSet<Product>()
                    .Where(x => x.Id == 44)
                    .Update(x => new Product()
                    {
                        Ext_F1 = (Convert.ToInt32(x.Ext_F1) + 1).ToString()
                    });

                var product = connection.QuerySet<Product>()
                                .Where(x => x.ProductCode == "ed761dea-c57d-4b9c-8000-e434fe280337" && x.IsDelete == false)
                                .Get(x => new GiftDto()
                                {
                                    ProductCode = x.ProductCode,
                                    Name = x.Name_CN,
                                    Stock = Convert.ToInt32(x.Ext_F1)
                                });

            }


        }

        private void Aop_OnExecuting(ref CommandDefinition command)
        {

        }
    }

    public class GlobalConfig
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public static string ConnectionString { get; set; } = "";

        /// <summary>
        /// 资源地址
        /// </summary>
        public static string ResourcesUrl { get; set; } = "";
    }
}
