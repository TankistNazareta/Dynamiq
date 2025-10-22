import IntervalEnum from '../../utils/enums/intervalEnum';
import { apiRequest } from '../api';

export type SubscriptionRes = {
    id: string;
    name: string;
    interval: IntervalEnum;
    price: number;
};

export const getAllSubsctiptions = async () => {
    return await apiRequest(`/subscription`, { method: 'GET' });
};
