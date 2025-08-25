import IntervalEnum from '../../utils/enums/intervalEnum';
import { apiRequest } from '../api';

export type PaymentHistoryRes = {
    id: string;
    amount: number;
    createdAt: Date;
    interval: IntervalEnum;
    products: ProductPaymentHistory[];
};

export type ProductPaymentHistory = {
    productId: string;
    quantity: number;
};

export const getPaymentHistoryByUserId = async (id: string) => {
    const res = await apiRequest<PaymentHistoryRes[]>(`/payment-history/by-id?id=${id}`);
    return res;
};

export const getPaymentHistoryByUserEmail = async (email: string) => {
    const res = await apiRequest<PaymentHistoryRes[]>(`/payment-history/by-email?email=${email}`);
    return res;
};
