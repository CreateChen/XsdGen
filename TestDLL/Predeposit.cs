using System;
using System.Collections.Generic;
using XsdAttribute;

[assembly: XsdSchema(
    TargetNamespace = "http://soa.ctrip.com/framework/soa/sample/v1",
    XmlNamespace = "http://soa.ctrip.com/framework/soa/sample/v1",
    Namespace = "http://soa.ctrip.com/framework/soa/sample/v1",
    Common = "http://soa.ctrip.com/common/types/v1")]
[assembly: XsdImport(Id = "SOACommonTypes",
    Namespace = "http://soa.ctrip.com/common/types/v1",
    SchemaLocation = "CtripSOACommonTypes_V1.0.0.xsd")]

namespace TestDLL
{
    [XsdComplexType(Annotation = "预存款申请", FileGroup = "PreDeposit")]
    public class PreDepositRequest
    {
        [XsdElement(MinOccurs = "1", Annotation = "供应商ID")]
        public int ProviderId { get; set; }

        [XsdElement(MinOccurs = "1", Annotation = "供应商名称")]
        public string ProviderName { get; set; }

        [XsdElement(Annotation = "建议付款日期")]
        public DateTime AdvisePayDate { get; set; }

        [XsdElement(MinOccurs = "0", MaxOccurs = "unbounded")]
        public List<int> Test { get; set; }

        [XsdElement(MinOccurs = "1", Annotation = "业务类型")]
        public BusinessType BusinessType { get; set; }
    }

    [XsdComplexType(Annotation = "预存款申请", FileGroup = "PreDeposit")]
    public class PreDepositResponse
    {
        public int BillId { get; set; }
    }

    [XsdSimpleType(Annotation = "业务类型", FileGroup = "BusinessType")]
    public enum BusinessType
    {
        预存款,
        保证金,
        预付款,
        押金
    }
}