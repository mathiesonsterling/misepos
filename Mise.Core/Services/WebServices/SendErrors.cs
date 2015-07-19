using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using System.Diagnostics.CodeAnalysis;

namespace Mise.Core.Services.WebServices
{
    public enum SendErrors
    {
        EmailAlreadyInUse,
        EmailLoggedInElsewhere,
        VendorAlreadyExists,
        Other
    }
    
}
