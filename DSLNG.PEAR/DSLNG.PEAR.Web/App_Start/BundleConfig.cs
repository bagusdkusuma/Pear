using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace DSLNG.PEAR.Web.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/jquery-ui.js",
                         "~/Scripts/moment.js",
                        "~/Content/bootstrap/js/bootstrap.js",
                //"~/Content/datepicker/js/bootstrap-datepicker.js",
                        "~/Scripts/bootstrap-datetimepicker.js",
                        "~/Content/select2/dist/js/select2.js",
                        "~/Content/select2/dist/js/select2.multi-checkboxes.js",
                        "~/Scripts/highcharts.js",
                        "~/Scripts/highcharts-3d.src.js",
                //"~/Scripts/highcharts-more.js",
                        "~/Scripts/exporting.js",
                        "~/Content/colpick/js/colpick.js",
                        "~/Scripts/snap.svg.js",
                        "~/Scripts/perfect-scrollbar.jquery.min.js",
                        "~/Scripts/trafficlight.js",
                        "~/Scripts/tank.js",
                        "~/Scripts/bignumber.js",
                        "~/Scripts/main.js",
                        "~/Scripts/calculator.constant.js",
                        "~/Scripts/calculator.plant-availability.js",
                        "~/Scripts/calculator.pricing.js",
                        "~/Scripts/calculator.production-yield.js",
                        "~/Scripts/calculator.standard.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/tableHeadFixer.js"
                       ));
            bundles.Add(new ScriptBundle("~/bundles/der_preview_js").Include(
                    "~/Scripts/jquery-1.11.3.js",
                    "~/Content/bootstrap/js/bootstrap.js",
                    "~/Scripts/moment.js",
                     "~/Scripts/bootstrap-datetimepicker.js",
                      "~/Scripts/highcharts.js",
                       "~/Scripts/snap.svg.js",
                        "~/Scripts/der-tank.js",
                         "~/Scripts/fabricjs.js",
                          "~/Scripts/der-scripts.js"
                ));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap/css/bootstrap.css",
                "~/Content/font-awesome/css/font-awesome.css",
                //"~/Content/datepicker/css/datepicker.css",
                  "~/Content/bootstrap-datetimepicker.css",
                  "~/Content/select2/dist/css/select2.css",
                  "~/Content/colpick/css/colpick.css",
                   "~/Content/perfect-scrollbar.min.css",
                  "~/Content/bootstrap.vertical-tabs.css",
                  "~/Content/style.css",
                  "~/Content/style-regawa.css",
                  "~/Content/datatables/jquery.dataTables.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/js_login").Include(
                         "~/Scripts/moment.js",
                        "~/Content/bootstrap/js/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css_login").Include("~/Content/bootstrap/css/bootstrap.css",
                "~/Content/font-awesome/css/font-awesome.css",
                "~/Content/login.css"));

            bundles.Add(new StyleBundle("~/Content/der_css").Include("~/Content/bootstrap/css/bootstrap.css",
              "~/Content/font-awesome/css/font-awesome.css",
                "~/Content/style-der.css"));

            bundles.Add(new StyleBundle("~/Content/der_preview").Include("~/Content/bootstrap/css/bootstrap.css",
                   "~/Content/font-awesome/css/font-awesome.css",
                   "~/Content/bootstrap/css/bootstrap.css",
                   "~/Content/bootstrap-datetimepicker.css",
                   "~/Content/der-dikdik.css"));



        }
    }
}