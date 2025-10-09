import IntervalEnum from '../../utils/enums/intervalEnum';
import { apiRequest } from '../api';

export type CreateCheckoutType = {
    url: string;
};

export type CreateCheckoutRequest =
    | {
          intervalEnum: IntervalEnum;
          successUrl: string;
          cancelUrl: string;
          userId: string;
          couponCodes?: string[];
      }
    | {
          productId: string;
          quantity: number;
          intervalEnum: IntervalEnum;
          successUrl: string;
          cancelUrl: string;
          userId: string;
      };

type CrateCheckoutProps =
    | {
          intervalEnum: IntervalEnum;
          userId: string;
          productId: string;
          quantity: number;
      }
    | {
          intervalEnum: IntervalEnum;
          userId: string;
          couponCodes?: string[];
      };

export const createCheckout = async (props: CrateCheckoutProps) => {
    alert('type: 4242 4242 4242 4242 for successfully payment, and 4000 0000 0000 0002 for cancel payment');
    const mainUrl = window.location.origin;

    let requestBody: CreateCheckoutRequest;

    if ('productId' in props && 'quantity' in props) {
        requestBody = {
            intervalEnum: props.intervalEnum,
            successUrl: `${mainUrl}/payment/success`,
            cancelUrl: `${mainUrl}/payment/failed`,
            userId: props.userId,
            productId: props.productId,
            quantity: props.quantity,
        };
    } else {
        requestBody = {
            intervalEnum: props.intervalEnum,
            successUrl: `${mainUrl}/payment/success`,
            cancelUrl: `${mainUrl}/payment/failed`,
            userId: props.userId,
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
