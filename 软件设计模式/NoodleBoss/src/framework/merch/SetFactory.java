package framework.merch;

import framework.merch.set.PorkNoodlesCoke;
import framework.merch.set.Set;
import framework.merch.set.ShrimpNoodlesCoke;
import framework.merch.set.VegetarianNoodlesCoke;
import framework.merch.single.SingleOrder;

public class SetFactory extends MerchFactory {
    @Override
    public SingleOrder createSingleOrder(MerchType merchType) {
        return null;
    }

    @Override
    public Set createSet(MerchType merchType) {
        switch (merchType) {
            case SET_PORK_NOODLES_COKE:
                return new PorkNoodlesCoke();
            case SET_SHRIMP_NOODLES_COKE:
                return new ShrimpNoodlesCoke();
            case SET_VEGETARIAN_NOODLES_COKE:
                return new VegetarianNoodlesCoke();
            default:
                throw new IllegalArgumentException("no such set in factory");
        }
    }
}
