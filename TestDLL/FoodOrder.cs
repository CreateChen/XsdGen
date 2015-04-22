using System;
using System.Collections.Generic;
using XsdAttribute;

[assembly: XsdSchema(
    TargetNamespace = "http://xx.com/framework/soa/sample/v1",
    XmlNamespace = "http://xx.com/framework/soa/sample/v1",
    Namespace = "http://xx.com/framework/soa/sample/v1",
    Common = "http://xx.com/common/types/v1")]
[assembly: XsdImport(Id = "SOACommonTypes",
    Namespace = "http://xx.com/common/types/v1",
    SchemaLocation = "SOACommonTypes_V1.0.0.xsd")]

namespace TestDLL
{
    [XsdComplexType(Annotation = "订餐申请", FileGroup = "FoodOrder")]
    public class FoodOrderRequest
    {
        [XsdElement(MinOccurs = "1", Annotation = "餐馆编号")]
        public int RestaurantId { get; set; }

        [XsdElement(MinOccurs = "1", Annotation = "餐馆名称")]
        public string RestaurantName { get; set; }

        [XsdElement(Annotation = "订餐日期")]
        public DateTime OrderDate { get; set; }

        [XsdElement(MinOccurs = "0", MaxOccurs = "unbounded", Annotation = "食品编号")]
        public List<int> FoodId { get; set; }

        [XsdElement(MinOccurs = "1", Annotation = "业务类型")]
        public PayType BusinessType { get; set; }
    }

    [XsdComplexType(Annotation = "订餐结果", FileGroup = "FoodOrder")]
    public class FoodOrderResponse
    {
        [XsdElement(MinOccurs = "1", Annotation = "订单编号")]
        public int OrderId { get; set; }

        [XsdElement(Annotation = "预计送达时间")]
        public DateTime DeliveryTime { get; set; }
    }

    [XsdSimpleType(Annotation = "付款类型", FileGroup = "PayType")]
    public enum PayType
    {
        现金,
        支付宝,
        微信,
        网银
    }
}