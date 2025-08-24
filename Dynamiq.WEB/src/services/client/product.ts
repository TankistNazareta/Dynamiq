import intervalEnum from '../../utils/enums/intervalEnum';
import SortEnum from '../../utils/enums/sortByEnum';
import { ApiResult } from '../../utils/types/api';
import { apiRequest } from '../api';

export interface ProductResBody {
    id: string;
    name: string;
    description: string;
    price: number;
    interval: intervalEnum;
    imgUrls: { imgUrl: string }[];
    paragraphs: { order: number; text: string }[];
    cardDescription: string;
    categoryId: string;
}

export interface ProductRes {
    totalCount: number;
    products: ProductResBody[];
}

export interface ProductFilter {
    categoryIds?: string[];
    minPrice?: number;
    maxPrice?: number;
    searchTerm?: string;
    sortBy?: SortEnum;
}

export const getRangeProduct = async (limit: number, offset: number) => {
    const res = await apiRequest<ProductRes>(`/product?limit=${limit}&offset=${offset}`);
    return res;
};

export const getByIdProduct = async (id: string) => {
    const res = await apiRequest<ProductResBody>(`/product/${id}`);
    return res;
};

export const getFilteredProducts = async (productFilter: ProductFilter, limit: number, offset: number) => {
    const res = await apiRequest<ProductRes>('/product/filter?limit=' + limit + '&offset=' + offset, {
        method: 'POST',
        body: JSON.stringify(productFilter),
    });

    return res;
};
