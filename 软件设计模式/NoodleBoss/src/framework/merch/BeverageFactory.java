package framework.merch;

import framework.merch.set.Set;
import framework.merch.single.SingleOrder;
import framework.merch.single.beverage.Coke;
import framework.merch.single.beverage.OrangeJuice;

class BeverageFactory extends MerchFactory {
    @Override
    public SingleOrder createSingleOrder(MerchType merchType) {
        switch (merchType) {
            case COKE:
                return new Coke(1.5);
            case ORANGE_JUICE:
                return new OrangeJuice(1.0);
            default:
                throw new IllegalArgumentException("no such beverage in factory");
        }
    }

    @Override
    public Set createSet(MerchType merchType) {
        return null;
    }
}
