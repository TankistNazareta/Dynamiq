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
        <div className="filter__section-item">
            <div
                className={`filter__section-item_parent d-flex align-items-center btn--rigth-arrow ${
                    needToShowChild ? 'btn--right-arrow--active' : ''
                }`}
                onClick={(e) => {
                    const target = e.target;

                    if (target instanceof HTMLInputElement && target.type === 'checkbox') return;

                    setNeedToShowChild((prev) => !prev);
                }}>
                {Array.from({ length: parent }, (_, i) => (
                    <hr key={i} className="filter__section-item__hr" />
                ))}

                <p className="filter__section-item__title">{name}</p>
                <input
                    type="checkbox"
                    checked={isChecked}
                    onChange={(e) => {
                        e.target.checked ? onSelect(id) : onRemove(id);
                    }}
                    className="filter__section-item__btn-checkbox"
                />
                {childrenCategories.length ? (
                    <button
                        className={`user__payment-history_item_descr user__payment-history_item_btn filter__section-item__btn-dropdown ${
                            needToShowChild ? 'user__payment-history_item__btn--active' : ''
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
