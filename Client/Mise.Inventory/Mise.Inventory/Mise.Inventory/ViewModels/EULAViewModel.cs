﻿using System;
using System.Threading.Tasks;
using Mise.Inventory.Services;
using Mise.Core.Services.UtilityServices;
using System.Windows.Input;
using Xamarin.Forms;
using Mise.Core.Client.Services;
namespace Mise.Inventory.ViewModels
{
    public class EULAViewModel : BaseViewModel
    {
        #region implemented abstract members of BaseViewModel
        public override Task OnAppearing()
        {
            return Task.FromResult(false);
        }
        #endregion

        private readonly ILoginService _loginService;
        public EULAViewModel(IAppNavigation appNavigation, ILogger logger, ILoginService loginService) 
            : base(appNavigation, logger){
            _loginService = loginService;
        }

        public bool HasAgreed{ get { return GetValue<bool>(); } set { SetValue(value); } }

        public ICommand AcceptCommand{ get { return new Command(Accept, () => HasAgreed); } }

        public async void Accept(){
            await _loginService.SetEulaAsShown();
            await Navigation.CloseEULA();
        }

        public string EULAText{
            get{
                return @"IMPORTANT, PLEASE READ CAREFULLY. 

THIS IS A LICENSE AGREEMENT. This SOFTWARE PRODUCT is protected by copyright laws and international copyright treaties, as well as other intellectual property laws and treaties. 

This SOFTWARE PRODUCT is licensed, not sold. 

End User License Agreement 

This End User License Agreement (""EULA"") is a legal agreement between you (either an individual or a single entity) and Mise Technologies, Inc. with regard to the copyrighted Software (herein referred to as ""SOFTWARE PRODUCT"" or ""SOFTWARE"") provided with this EULA. The SOFTWARE PRODUCT includes computer software, the associated media, any printed materials, and any ""online"" or electronic documentation. Use of any software and related documentation (""Software"") provided to you by Mise Technologies, Inc.  in whatever form or media, will constitute your acceptance of these terms, unless separate terms are provided by the software supplier, in which case certain additional or different terms may apply. If you do not agree with the terms of this EULA, do not download, install, copy or use the Software. By installing, copying or otherwise using the SOFTWARE PRODUCT, you agree to be bound by the terms of this EULA. If you do not agree to the terms of this EULA, Mise Technologies, Inc. is unwilling to license the SOFTWARE PRODUCT to you. 

1. Eligible Licensees. This Software is available for license solely to SOFTWARE owners, with no right of duplication or further distribution, licensing, or sub-licensing. IF YOU DO NOT OWN THE SOFTWARE, THEN DO NOT DOWNLOAD, INSTALL, COPY OR USE THE SOFTWARE. 
2. License Grant. Mise Technologies, Inc. grants to you a personal, non-transferable and non-exclusive right to use the copy of the Software provided with this EULA. You agree you will not copy the Software except as necessary to use it on a single computer. You agree that you may not copy the written materials accompanying the Software. Modifying, translating, renting, copying, transferring or assigning all or part of the Software, or any rights granted hereunder, to any other persons and removing any proprietary notices, labels or marks from the Software is strictly prohibited. Furthermore, you hereby agree not to create derivative works based on the Software. You may not transfer this Software. 
3. Copyright. The Software is licensed, not sold. You acknowledge that no title to the intellectual property in the Software is transferred to you. You further acknowledge that title and full ownership rights to the Software will remain the exclusive property of Mise Technologies, Inc.  and/or its suppliers, and you will not acquire any rights to the Software, except as expressly set forth above. All copies of the Software will contain the same proprietary notices as contained in or on the Software. All title and copyrights in and to the SOFTWARE PRODUCT (including but not limited to any images, photographs, animations, video, audio, music, text and ""applets,"" incorporated into the SOFTWARE PRODUCT), the accompanying printed materials, and any copies of the SOFTWARE PRODUCT, are owned by Mise Technologies, Inc. or its suppliers. The SOFTWARE PRODUCT is protected by copyright laws and international treaty provisions. You may not copy the printed materials accompanying the SOFTWARE PRODUCT. 
4. Reverse Engineering. You agree that you will not attempt, and if you are a corporation, you will use your best efforts to prevent your employees and contractors from attempting to reverse compile, modify, translate or disassemble the Software in whole or in part. Any failure to comply with the above or any other terms and conditions contained herein will result in the automatic termination of this license and the reversion of the rights granted hereunder to Mise Technologies, Inc.
5. Disclaimer of Warranty. The Software is provided ""AS IS"" without warranty of any kind. Mise Technologies, Inc.  and its suppliers disclaim and make no express or implied warranties and specifically disclaim the warranties of merchantability, fitness for a particular purpose and non-infringement of third-party rights. The entire risk as to the quality and performance of the Software is with you. NeitherMise Technologies, Inc.  nor its suppliers warrant that the functions contained in the Software will meet your requirements or that the operation of the Software will be uninterrupted or error-free. Mise Technologies, Inc.  IS NOT OBLIGATED TO PROVIDE ANY UPDATES TO THE SOFTWARE. 
6. Limitation of Liability. Mise Technologies, Inc. 's entire liability and your exclusive remedy under this EULA shall not exceed the price paid for the Software, if any. In no event shall Mise Technologies, Inc.  or its suppliers be liable to you for any consequential, special, incidental or indirect damages of any kind arising out of the use or inability to use the software, even if Mise Technologies, Inc.  or its supplier has been advised of the possibility of such damages, or any claim by a third party. 
7. Rental. You may not loan, rent, or lease the SOFTWARE. 
8. Upgrades. If the SOFTWARE is an upgrade from an earlier release or previously released version, you now may use that upgraded product only in accordance with this EULA. If the SOFTWARE PRODUCT is an upgrade of a software program which you licensed as a single product, the SOFTWARE PRODUCT may be used only as part of that single product package and may not be separated for use on more than one computer. 
9. OEM Product Support. Product support for the SOFTWARE PRODUCT IS provided by Mise Technologies, Inc. . For product support, please call Mise Technologies, Inc. . Should you have any questions concerning this, please refer to the address provided in the documentation. 
10. No Liability for Consequential Damages. In no event shall Mise Technologies, Inc.  or its suppliers be liable for any damages whatsoever (including, without limitation, incidental, direct, indirect special and consequential damages, damages for loss of business profits, business interruption, loss of business information, or other pecuniary loss) arising out of the use or inability to use this ""Your Company"" product, even if Mise Technologies, Inc.  has been advised of the possibility of such damages. Because some states/countries do not allow the exclusion or limitation of liability for consequential or incidental damages, the above limitation may not apply to you.
11. Use of Data.  You agree that Mise Technologies may use information from your use of the product in aggregate, and transfer this information to partners. 
12. Indemnification By You. If you distribute the Software in violation of this Agreement, you agree to indemnify, hold harmless and defend Mise Technologies, Inc. and its suppliers from and against any claims or lawsuits, including attorney's fees that arise or result from the use or distribution of the Software in violation of this Agreement. 

Mise Technologies, Inc.  
522 Lefferts Ave. #2G
Brooklyn, NY 11225
Phone: 917-687-8931 / 718-715-2945
Email: info@misepos.com


";
            }
        }
    }
}

