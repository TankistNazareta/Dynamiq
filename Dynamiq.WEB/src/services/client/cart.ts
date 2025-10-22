import { apiRequest } from '../api';

export type CartRes = {
    items: { productId: string; quantity: number }[];
};

export const getCart = async () => {
    const res = await apiRequest<CartRes>(`/cart`, {
        method: 'GET',
    });
    return res;
};

export const setQuantityCartItem = async (productId: string, quantity: number) => {
    const res = await apiRequest<CartRes>(`/cart?productId=${productId}&quantity=${quantity}`, {
        method: 'PUT',
    });
    return res;
};

export const clearCart = async () => {
    const res = await apiRequest<void>(`/cart`, {
        method: 'DELETE',
    });
    return res;
};

export const addQuantityToCartItem = async (productId: string, quantity: number) => {
    const res = await apiRequest<CartRes>(`/cart?productId=${productId}&quantity=${quantity}`, {
        method: 'POST',
    });
    return res;
};
