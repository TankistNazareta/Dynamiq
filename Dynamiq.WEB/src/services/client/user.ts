import IntervalEnum from '../../utils/enums/intervalEnum';
import roleEnum from '../../utils/enums/roleEnum';
import { apiRequest } from '../api';
import { PaymentHistoryRes } from './paymentHistory';

export type UserRes = {
    id: string;
    email: string;
    role: roleEnum;
    emailVerification: {
        createdAt: Date;
        isConfirmed: boolean;
    };
    paymnetHistories: PaymentHistoryRes[];
    subscription: { isConfirmed: boolean };
};

export const getUserById = async (id: string) => {
    const res = await apiRequest<UserRes>(`/user?id=${id}`);
    return res;
};

export const getUserByEmail = async (email: string) => {
    const res = await apiRequest<UserRes>(`/user/email?email=${email}`);
    return res;
};
