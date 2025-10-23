import { apiRequest } from '../api';

export type CreateCheckoutType = {
    url: string;
};

export type CreateCheckoutRequest =
    | {
          successUrl: string;
          cancelUrl: string;
          couponCodes?: string[];
      }
    | {
          productId: string;
          quantity: number;
          successUrl: string;
          cancelUrl: string;
      }
    | {
          subscriptionId: string;
          successUrl: string;
          cancelUrl: string;
      };

type CreateCheckoutProps =
    | {
          productId: string;
          quantity: number;
      }
    | {
          couponCodes?: string[];
      }
    | {
          subscriptionId: string;
      };

export const createCheckout = async (props: CreateCheckoutProps) => {
    alert('type: 4242 4242 4242 4242 for successfully payment, and 4000 0000 0000 0002 for fail payment');
    const mainUrl = window.location.origin;

    let requestBody: CreateCheckoutRequest;

    if ('productId' in props && 'quantity' in props) {
        requestBody = {
            successUrl: `${mainUrl}/payment/success`,
            cancelUrl: `${mainUrl}/payment/failed`,
            productId: props.productId,
            quantity: props.quantity,
        };
    } else if ('subscriptionId' in props) {
        requestBody = {
            successUrl: `${mainUrl}/payment/success`,
            cancelUrl: `${mainUrl}/payment/failed`,
            subscriptionId: props.subscriptionId,
        };
    } else {
        requestBody = {
            successUrl: `${mainUrl}/payment/success`,
            cancelUrl: `${mainUrl}/payment/failed`,
            couponCodes: props.couponCodes,
        };
    }

    const res = await apiRequest<CreateCheckoutType>(`/payment/create-checkout-session`, {
        body: JSON.stringify(requestBody),
        method: 'POST',
    });

    if (res.success && res.data) {
        window.location.href = res.data.url;
    }
    return res;
};
