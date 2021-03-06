﻿using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using Web.MyOffice.Data;
using Web.MyOffice.Models;
using System.Web;

namespace ASE
{
    public class PrivatBankRatesSource : ICurrencyRateSource
    {
        private class PrivateBankCurrency
        {
            public string base_ccy { set; get; }
            public string ccy { set; get; }
            public decimal buy { set; get; }
            public decimal sell { set; get; }
        }
        public PrivatBankRatesSource()
        {
            Id = Guid.NewGuid();
        }
        private bool isInited = false;
        public PrivatBankRatesSource(string name, Uri baseSource, Dictionary<string, string> routeParams)
        {
            BaseSource = baseSource;
            RouteParams = routeParams;
            if(routeParams != null && routeParams.Count > 0)
            {
                RouteParamString = routeParams.ToList().Aggregate("?", (key, value) => key + "=" + value + "&");
            }
            else
            {
                RouteParamString = string.Empty;
            }
            BaseCurrency = CurrencyType.UAH;
            Name = name;
            isInited = true;
            Id = Guid.NewGuid();
        }
        public string RouteParamString { set; get; }
        public Uri BaseSource { set; get; }
        public Guid Id { set; get; }
        public string Name { set; get; }
        public Dictionary<string,string> RouteParams {  set; get; }
        public Dictionary<CurrencyType, decimal> LoadedRates { set; get; }
        public CurrencyType BaseCurrency { set; get; }
        public bool Load(List<string> userTypes)
        {
            if (isInited)
            {
                using (var wc = new WebClient())
                {
                    var stringRawData = wc.DownloadString(BaseSource.AbsoluteUri + RouteParamString);
                    var loadedRates = JsonConvert.DeserializeObject<PrivateBankCurrency[]>(stringRawData).ToList();
                    LoadedRates = loadedRates.Join(userTypes.Distinct(),
                        rate => rate.ccy,
                        type => type,
                        (rate, type) =>
                        new KeyValuePair<string, decimal>(type, rate.buy))
                        .ToDictionary(KeyValuePair => (CurrencyType)Enum.Parse(typeof(CurrencyType), KeyValuePair.Key), KeyValuePair => KeyValuePair.Value);
                }
                return true;
            }
            else return false;
        }
    }
}