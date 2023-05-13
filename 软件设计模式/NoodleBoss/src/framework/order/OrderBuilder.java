package framework.order;

import framework.merch.Merch;
import framework.merch.MerchFactory;
import framework.merch.MerchFactoryMaker;
import framework.merch.MerchType;

import java.util.ArrayList;

/**
 * DP: Builder
 * 流程：新建 builder 对象
 * 点单
 * 确认订单
 */
public class OrderBuilder {

    private ArrayList<Merch> merches = new ArrayList<>();


    /**
     * 点面条
     *
     * @param type 面条类型
     */
    public void orderNoodles(MerchType type) {
        try {
            MerchFactory factory = MerchFactoryMaker.create(MerchFactoryMaker.MerchFactoryType.NOODLES);
            merches.add(factory.createSingleOrder(type));
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
    }

    /**
     * 点饮料
     *
     * @param type 饮料类型
     */
    public void orderBeverage(MerchType type) {
        try {
            MerchFactory factory = MerchFactoryMaker.create(MerchFactoryMaker.MerchFactoryType.BEVERAGE);
            merches.add(factory.createSingleOrder(type));
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
    }

    /**
     * 点套餐
     *
     * @param type 套餐类型
     */
    public void orderSet(MerchType type) {
        try {
            MerchFactory factory = MerchFactoryMaker.create(MerchFactoryMaker.MerchFactoryType.SET);
            merches.add(factory.createSet(type));
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
    }

    /**
     * 下订单
     *
     * @return 返回订单对象
     */
    public Order order() {
        Order order = new Order();

        for (Merch merch : merches) {
            order.add(merch);
        }

        return order;
    }
}
