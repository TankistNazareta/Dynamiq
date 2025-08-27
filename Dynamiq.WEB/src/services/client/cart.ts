import { apiRequest } from '../api';

export type CartRes = {
    items: { productId: string; quantity: number }[];
};

export const addItemToCart = async (userId: string, productId: string, quantity: number) => {
    const res = await apiRequest<CartRes>(`/cart?userId=${userId}&productId=${productId}&quantity=${quantity}`, {
        method: 'POST',
    });
    return res;
};

export const getCart = async (userId: string) => {
    const res = await apiRequest<CartRes>(`/cart?userId=${userId}`, {
        method: 'GET',
    });
    return res;
};

export const removeItemFromCart = async (userId: string, productId: string, quantity: number) => {
    const res = await apiRequest<CartRes>(`/cart?userId=${userId}&productId=${productId}&quantity=${quantity}`, {
        method: 'PUT',
    });
    return res;
};

export const clearCart = async (userId: string) => {
    const res = await apiRequest<void>(`/cart?userId=${userId}`, {
        method: 'DELETE',
    });
    return res;
};
