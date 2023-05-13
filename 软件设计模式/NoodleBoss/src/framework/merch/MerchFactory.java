package framework.merch;

import framework.merch.set.Set;
import framework.merch.single.SingleOrder;

public abstract class MerchFactory {
    public abstract SingleOrder createSingleOrder(MerchType merchType);

    public abstract Set createSet(MerchType merchType);
}
