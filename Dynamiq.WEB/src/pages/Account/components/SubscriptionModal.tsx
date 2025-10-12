import React, { useEffect, useState } from 'react';
import '../scss/subscriptionModal.scss';
import { useHttp } from '../../../hooks/useHttp/HttpContext';
import { getSubscriptions, ProductRes, ProductResBody } from '../../../services/client/product';
import IntervalEnum from '../../../utils/enums/intervalEnum';
import Loading from '../../../components/Loading';

interface SubscriptionModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export default function SubscriptionModal({ isOpen, onClose }: SubscriptionModalProps) {
    const [selectedPlan, setSelectedPlan] = useState<IntervalEnum>();
    const [purchasing, setPurchasing] = useState(false);
    const [subscrioptions, setSubscrioptions] = useState<ProductResBody[]>();

    const { state, makeRequest, setState } = useHttp();

    useEffect(() => {
        makeRequest<ProductRes>(() => getSubscriptions())
            .then((res) => {
                setSubscrioptions(res.products);
                setState('success');
            })
            .catch(() => {});
    }, []);

    const formatPrice = (value: number) => {
        try {
            return new Intl.NumberFormat(undefined, { style: 'currency', currency: '$' }).format(value);
        } catch {
            return `${value} $`;
        }
    };

    const handleBuy = async () => {
        if (purchasing) return;
        setPurchasing(true);
        try {
            // await onPurchase(selectedPlan);
        } finally {
            setPurchasing(false);
        }
    };

    const monthlyPrice = subscrioptions?.find((item) => item.interval === IntervalEnum.Monthly)?.price ?? 0;
    const yearlyPrice = subscrioptions?.find((item) => item.interval === IntervalEnum.Yearly)?.price ?? 0;

    if (!isOpen) return null;

    return (
        <div className="subscription-modal">
            <div className="subscription-modal__backdrop" onClick={onClose} />

            <div className="subscription-modal__dialog">
                <div className="subscription-modal__content">
                    {state === 'success' ? (
                        <>
                            {' '}
                            <div className="subscription-modal__header">
                                <h5 className="subscription-modal__title">Choose Your Plan</h5>
                                <button type="button" className="subscription-modal__close" onClick={onClose} />
                            </div>
                            <div className="subscription-modal__body">
                                <p className="subscription-modal__lead">
                                    Select a monthly or yearly subscription. Save more with the yearly plan.
                                </p>

                                <div className="subscription-modal__plans">
                                    <label
                                        className={`subscription-modal__plan ${
                                            selectedPlan === IntervalEnum.Monthly
                                                ? 'subscription-modal__plan--selected'
                                                : ''
                                        }`}
                                        onClick={() => setSelectedPlan(IntervalEnum.Monthly)}>
                                        <input
                                            type="radio"
                                            name="subscription-plan"
                                            checked={selectedPlan === IntervalEnum.Monthly}
                                            onChange={() => setSelectedPlan(IntervalEnum.Monthly)}
                                            className="d-none"
                                        />
                                        <div className="subscription-modal__plan-head">
                                            <div className="subscription-modal__plan-name">Monthly</div>
                                            <div className="subscription-modal__plan-price">
                                                {formatPrice(monthlyPrice)} / month
                                            </div>
                                        </div>
                                        <div className="subscription-modal__plan-body">Cancel anytime.</div>
                                    </label>

                                    <label
                                        className={`subscription-modal__plan ${
                                            selectedPlan === IntervalEnum.Yearly
                                                ? 'subscription-modal__plan--selected'
                                                : ''
                                        }`}
                                        onClick={() => setSelectedPlan(IntervalEnum.Yearly)}>
                                        <input
                                            type="radio"
                                            name="subscription-plan"
                                            checked={selectedPlan === IntervalEnum.Yearly}
                                            onChange={() => setSelectedPlan(IntervalEnum.Yearly)}
                                            className="d-none"
                                        />
                                        <div className="subscription-modal__plan-head">
                                            <div className="subscription-modal__plan-name">Yearly</div>
                                            <div className="subscription-modal__plan-price">
                                                {formatPrice(yearlyPrice)} / year
                                            </div>
                                        </div>
                                        <div className="subscription-modal__plan-body">
                                            Save more compared to monthly.
                                        </div>
                                    </label>
                                </div>

                                <div className="subscription-modal__summary">
                                    <div className="subscription-modal__summary-line">
                                        Selected:{' '}
                                        <span>{selectedPlan === IntervalEnum.Monthly ? 'Monthly' : 'Yearly'}</span>
                                    </div>
                                    <div className="subscription-modal__summary-line">
                                        Price:{' '}
                                        <span>
                                            {selectedPlan === IntervalEnum.Monthly
                                                ? formatPrice(monthlyPrice) + ' / month'
                                                : formatPrice(yearlyPrice) + ' / year'}
                                        </span>
                                    </div>
                                </div>
                            </div>
                            <div className="subscription-modal__footer">
                                <button
                                    type="button"
                                    className="subscription-modal__cancel"
                                    onClick={onClose}
                                    disabled={purchasing}>
                                    Cancel
                                </button>
                                <button
                                    type="button"
                                    className="subscription-modal__buy"
                                    onClick={handleBuy}
                                    disabled={purchasing}>
                                    {purchasing
                                        ? 'Processing...'
                                        : `Buy — ${
                                              selectedPlan === IntervalEnum.Monthly
                                                  ? formatPrice(monthlyPrice) + ' / month'
                                                  : formatPrice(yearlyPrice) + ' / year'
                                          }`}
                                </button>
                            </div>
                        </>
                    ) : (
                        <Loading />
                    )}
                </div>
            </div>
        </div>
    );
}
