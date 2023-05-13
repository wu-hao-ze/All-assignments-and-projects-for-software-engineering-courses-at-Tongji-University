package framework.merch;

import framework.merch.set.NullSet;
import framework.merch.set.Set;
import framework.merch.single.NullSingleOrder;
import framework.merch.single.SingleOrder;

/**
 * DP: Null Object
 */
public class NullMerchFactory extends MerchFactory {
    @Override
    public SingleOrder createSingleOrder(MerchType merchType) {
        return new NullSingleOrder();
    }

    @Override
    public Set createSet(MerchType merchType) {
        return new NullSet();
    }
}
