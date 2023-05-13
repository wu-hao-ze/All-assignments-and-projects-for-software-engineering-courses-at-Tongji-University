package framework.merch.set;

import framework.merch.MerchFactoryMaker;
import framework.merch.MerchType;
import framework.merch.single.SingleOrder;

import java.util.ArrayList;
import java.util.Arrays;

public class PorkNoodlesCoke extends Set {
    public PorkNoodlesCoke() {
        super(new ArrayList<SingleOrder>(Arrays.asList(
                MerchFactoryMaker.create(MerchFactoryMaker.MerchFactoryType.NOODLES).createSingleOrder(MerchType.PORK_NOODLES),
                MerchFactoryMaker.create(MerchFactoryMaker.MerchFactoryType.BEVERAGE).createSingleOrder(MerchType.COKE)
        )));
    }

    @Override
    public String getName() {
        return "大排面套餐";
    }
}
