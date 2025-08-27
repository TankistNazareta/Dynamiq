import DiscountTypeEnum from '../../utils/enums/discountTypeEnum';
import { apiRequest } from '../api';

export type CouponRes = {
    id: string;
    code: string;
    discountType: DiscountTypeEnum;
    discountValue: number;
    startTime: Date;
    endTime: Date;
    isActiveCoupon: boolean;
};

export const getCoupon = async (code: string) => {
    const res = await apiRequest<CouponRes>(`/coupon?code=${code}`);
    return res;
};
