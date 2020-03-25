using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Identity.Client;

namespace AndroidApp
{
    [Activity]
    [IntentFilter(new[] { Intent.ActionView },
      Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
      DataHost = "com.companyname.androidapp",
      DataScheme = "msauth",
      DataPath = "/DWxBsg/Q8zSqNAzwnqv6YIZbJr4=")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}