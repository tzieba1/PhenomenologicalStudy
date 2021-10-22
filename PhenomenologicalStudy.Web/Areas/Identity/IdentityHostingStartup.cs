using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Data;

[assembly: HostingStartup(typeof(PhenomenologicalStudy.Web.Areas.Identity.IdentityHostingStartup))]
namespace PhenomenologicalStudy.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}