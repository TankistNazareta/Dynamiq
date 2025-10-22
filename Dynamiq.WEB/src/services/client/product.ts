import SortEnum from '../../utils/enums/sortByEnum';
import { apiRequest } from '../api';

export interface ProductResBody {
    id: string;
    name: string;
    description: string;
    price: number;
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

export interface SearchedNamesRes {
    names: string[];
}

export const getSearchedNames = async (productFilter: ProductFilter) => {
    const res = await apiRequest<SearchedNamesRes>(`/product/search-names?limit=${5}`, {
        body: JSON.stringify(productFilter),
        method: 'POST',
    });
    return res;
};

export const addViewCount = async (id: string) => {
    await apiRequest(`/product/add-view?productId=${id}`, { method: 'PUT' });
};

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
