﻿using WYF.DataEntity.Entity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Form.container;

namespace WYF.Form.control
{
    /// <summary>
    /// 内含搜索框和搜索按钮
    /// </summary>
    public class Search: Container
    {
        /// <summary>
        /// 搜索风格
        /// </summary>
        [SimpleProperty]
        public int ShowModel { get; set; }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        //public string SearchKey 
        //{
        //    get { return this.clientViewProxy.view }
        //}
    }
}
