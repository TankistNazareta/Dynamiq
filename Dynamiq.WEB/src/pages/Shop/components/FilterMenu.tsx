import { ReactElement, useEffect, useState } from 'react';
import SortEnum from '../../../utils/enums/sortByEnum';
import { ProductFilter } from '../../../services/client/product';
import CategoryItem, { CategoryItemPorps } from './CategoryItem';
import useHttpHook from '../../../hooks/useHttp';
import { CategoryRes, getAllCategories } from '../../../services/client/category';
import { ErrorMsgType } from '../../../utils/types/api';
import Loading from '../../../components/Loading';
import { CloseButton } from 'react-bootstrap';
import { useLocation, useSearchParams } from 'react-router-dom';
import getFilterFromUrl from '../../../utils/services/getFilterFromUrl';

interface FilterMenuProps {
    isActive: boolean;
    onFilterProp: (filter: ProductFilter) => void;
    setNeedShowFilterMenu: (needToShow: boolean) => void;
}

const FilterMenu: React.FC<FilterMenuProps> = ({ isActive, onFilterProp, setNeedShowFilterMenu }) => {
    const [error, setError] = useState('');
    const [filter, setFilter] = useState<ProductFilter>({});
    const [categoryItems, setCategoryItems] = useState<CategoryItemPorps[]>([]);
    const [needToSetFilterFromUrl, setNeedToSetFilterFromUrl] = useState(false);

    const { state, setState, makeRequest } = useHttpHook();

    const [searchParams, setSearchParams] = useSearchParams();
    const location = useLocation();

    useEffect(() => {
        updateFilterFromUrl();
    }, [location.search]);

    useEffect(() => {
        if (!categoryItems.length) {
            makeRequest<CategoryRes[]>(() => getAllCategories())
                .then((res: CategoryRes[]) => {
                    const data = res.map((category) => createDataForChildrenCategoryFromData(category, 1));
                    setNeedToSetFilterFromUrl(true);
                    setCategoryItems(data);
                })
                .then(() => setState('success'))
                .catch((error: ErrorMsgType) => setError(error.Message));
        }
    }, []);

    const updateFilterFromUrl = () => {
        var newFilter = getFilterFromUrl(searchParams, categoryItems);

        if (newFilter.categoryIds) newFilter.categoryIds.forEach((id) => setCheckToCategory(id, true));

        onFilterProp(newFilter);
    };

    useEffect(() => {
        if (!needToSetFilterFromUrl) return;

        setNeedToSetFilterFromUrl(false);

        updateFilterFromUrl();
    }, [categoryItems]);

    const createDataForChildrenCategoryFromData = (category: CategoryRes, parentNumber: number): CategoryItemPorps => {
        const categoryProp: CategoryItemPorps = {
            id: category.id,
            name: category.name,
            onRemove: (id: string) => {
                setCheckToCategory(id, false);
            },

            onSelect: (id: string) => {
                setCheckToCategory(id, true);
            },

            isChecked: false,
            parent: parentNumber,
            subCategories: [],
        };

        if (category.subCategories && category.subCategories.length !== 0) {
            const res = category.subCategories.map((subCategory) =>
                createDataForChildrenCategoryFromData(subCategory, parentNumber + 1)
            );

            categoryProp.subCategories = [...categoryProp.subCategories, ...res];
        }

        return categoryProp;
    };

    const updateCategoriesChecked = (
        categories: CategoryItemPorps[],
        id: string,
        isChecked: boolean
    ): CategoryItemPorps[] => {
        const setDescendants = (cat: CategoryItemPorps, value: boolean): CategoryItemPorps => {
            const children = (cat.subCategories || []).map((c) => setDescendants(c, value));
            return { ...cat, isChecked: value, subCategories: children };
        };

        const mapAndUpdate = (cats: CategoryItemPorps[]): CategoryItemPorps[] => {
            return cats.map((cat) => {
                if (cat.id === id) return setDescendants(cat, isChecked);
                if (cat.subCategories && cat.subCategories.length) {
                    const newChildren = mapAndUpdate(cat.subCategories);
                    const newIsChecked = newChildren.length ? newChildren.every((c) => c.isChecked) : cat.isChecked;
                    return { ...cat, subCategories: newChildren, isChecked: newIsChecked };
                }
                return cat;
            });
        };

        return mapAndUpdate(categories);
    };

    const setCheckToCategory = (id: string, isChecked: boolean) => {
        setCategoryItems((prev) => updateCategoriesChecked(prev, id, isChecked));
    };

    const GetCheckedCategoriesForUrl = (categories: CategoryItemPorps[] = categoryItems) => {
        const idsCategories: string[] = [];

        for (const category of categories) {
            if (category.isChecked) idsCategories.push(category.id);
            else if (category.subCategories.length)
                idsCategories.push(...GetCheckedCategoriesForUrl(category.subCategories));
        }

        return idsCategories;
    };

    const onFilter = () => {
        if (filter.minPrice !== undefined && filter.maxPrice !== undefined && filter.minPrice > filter.maxPrice) {
            setError('min price cannot be bigger than max price');
            return null;
        }

        setError('');

        // url
        const searchParams = new URLSearchParams();

        if (filter.sortBy !== undefined && filter.sortBy !== SortEnum.Default)
            searchParams.set('sortBy', filter.sortBy!.toString());
        else searchParams.delete('sortBy');

        if (filter.maxPrice) searchParams.set('maxPrice', filter.maxPrice.toString());
        else searchParams.delete('maxPrice');

        if (filter.minPrice) searchParams.set('minPrice', filter.minPrice.toString());
        else searchParams.delete('minPrice');

        const checkedCategoriesForUrl = GetCheckedCategoriesForUrl();

        searchParams.delete('category');

        if (checkedCategoriesForUrl.length) {
            checkedCategoriesForUrl.forEach((c) => searchParams.append('category', c));
        }

        setSearchParams(searchParams);
    };

    const GetCheckedCategories = (categories: CategoryItemPorps[] = categoryItems): string[] => {
        const idsCategories: string[] = [];

        for (const category of categories) {
            if (category.subCategories.length) idsCategories.push(...GetCheckedCategories(category.subCategories));

            if (category.isChecked) idsCategories.push(category.id);
        }

        return idsCategories;
    };

    return (
        <div className={`shop__filter ${isActive ? 'shop__filter-active' : ''}`}>
            <CloseButton onClick={() => setNeedShowFilterMenu(false)} className="shop__filter_btn-close" />
            <h3 className="shop__filter_title">Filter</h3>
            <div className="shop__filter__section">
                <h5 className="shop__filter__section_title">Sort by:</h5>
                <div className="shop__filter__section__item d-flex align-items-center">
                    <hr className="shop__filter__section__item_hr" />
                    <p className="shop__filter__section__item_title">From lowest price</p>
                    <input
                        type="checkbox"
                        checked={filter.sortBy === SortEnum.FromLowest}
                        onChange={(e) =>
                            setFilter((prev) => ({
                                ...prev,
                                sortBy: e.target.checked ? SortEnum.FromLowest : SortEnum.Default,
                            }))
                        }
                        className="shop__filter__section__item_btn-checkbox"
                    />
                </div>
                <div className="shop__filter__section__item d-flex align-items-center">
                    <hr className="shop__filter__section__item_hr" />
                    <p className="shop__filter__section__item_title">From highest price</p>
                    <input
                        type="checkbox"
                        checked={filter.sortBy === SortEnum.FromHightest}
                        onChange={(e) =>
                            setFilter((prev) => ({
                                ...prev,
                                sortBy: e.target.checked ? SortEnum.FromHightest : SortEnum.Default,
                            }))
                        }
                        className="shop__filter__section__item_btn-checkbox"
                    />
                </div>
            </div>
            <hr className="hr-separator" />
            <div className="shop__filter__section">
                <h5 className="shop__filter__section_title">Filter:</h5>
                <div className="shop__filter__section__item d-flex align-items-center">
                    <hr className="shop__filter__section__item_hr" />
                    <p className="shop__filter__section__item_title">Highest price</p>
                    <input
                        type="text"
                        value={filter.maxPrice ?? 0}
                        onChange={(e) => {
                            const onlyNums = e.target.value.replace(/\D/g, '');
                            setFilter((prev) => ({
                                ...prev,
                                maxPrice: e.target.value ? Number.parseInt(onlyNums) : undefined,
                            }));
                        }}
                        className="shop__filter__section__item_btn-input"
                    />
                </div>
                <div className="shop__filter__section__item d-flex align-items-center">
                    <hr className="shop__filter__section__item_hr" />
                    <p className="shop__filter__section__item_title">Lowest price</p>
                    <input
                        type="text"
                        value={filter.minPrice ?? 0}
                        onChange={(e) => {
                            const onlyNums = e.target.value.replace(/\D/g, '');
                            setFilter((prev) => ({
                                ...prev,
                                minPrice: e.target.value ? Number.parseInt(onlyNums) : undefined,
                            }));
                        }}
                        className="shop__filter__section__item_btn-input"
                    />
                </div>
            </div>
            <hr className="hr-separator" />
            <div className="shop__filter__section">
                <h5 className="shop__filter__section_title">Categories:</h5>
                {state == 'loading' ? (
                    <Loading />
                ) : state == 'success' ? (
                    <>
                        {categoryItems?.map((item) => (
                            <CategoryItem key={item.id} {...item} />
                        ))}
                    </>
                ) : (
                    ''
                )}
            </div>
            <div className="shop__filter__section-footer">
                <button className="shop__filter__section-footer_btn" onClick={() => onFilter()}>
                    Search
                </button>
                <h3 className="shop__filter__section-footer_error text-danger">
                    {error ? 'error while getting categories: ' + error : ''}
                </h3>
            </div>
        </div>
    );
};

export default FilterMenu;
