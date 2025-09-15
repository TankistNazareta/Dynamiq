import { ReactElement, useEffect, useState } from 'react';

export interface CategoryItemPorps {
    id: string;
    name: string;
    subCategories: CategoryItemPorps[];
    onRemove: (id: string) => void;
    onSelect: (id: string) => void;
    isChecked: boolean;
    parent: number;
}

const CategoryItem: React.FC<CategoryItemPorps> = ({
    id,
    name,
    subCategories: childrenCategories,
    onRemove,
    onSelect,
    isChecked,
    parent = 1,
}) => {
    const [needToShowChild, setNeedToShowChild] = useState(false);

    return (
        <div className="shop__filter__section__item">
            <div
                className="shop__filter__section__item_parent d-flex align-items-center"
                onClick={(e) => {
                    const target = e.target;

                    if (target instanceof HTMLInputElement && target.type === 'checkbox') return;

                    setNeedToShowChild((prev) => !prev);
                }}>
                {Array.from({ length: parent }, (_, i) => (
                    <hr key={i} className="shop__filter__section__item_hr" />
                ))}

                <p className="shop__filter__section__item_title">{name}</p>
                <input
                    type="checkbox"
                    checked={isChecked}
                    onChange={(e) => {
                        e.target.checked ? onSelect(id) : onRemove(id);
                    }}
                    className="shop__filter__section__item_btn-checkbox"
                />
                {childrenCategories.length ? (
                    <button
                        className={`user__payment-history_item_descr user__payment-history_item_btn shop__filter__section__item_btn-dropdown ${
                            needToShowChild ? 'user__payment-history_item_btn-active' : ''
                        }`}>
                        <span></span>
                        <span></span>
                    </button>
                ) : null}
            </div>
            {needToShowChild &&
                childrenCategories &&
                childrenCategories.map((item) => (
                    <CategoryItem
                        key={item.id}
                        id={item.id}
                        name={item.name}
                        subCategories={item.subCategories}
                        onRemove={onRemove}
                        onSelect={onSelect}
                        isChecked={item.isChecked}
                        parent={parent + 1}
                    />
                ))}
        </div>
    );
};

export default CategoryItem;
