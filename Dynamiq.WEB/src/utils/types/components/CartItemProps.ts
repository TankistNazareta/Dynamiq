interface CartItemProps {
    imgUrl: string;
    name: string;
    quantity: number;
    priceTotal: number;
    onChangeQuantity: (quantity: number) => void;
    onDelete: () => void;
    onSetQuantity: (quantity: number) => void;
    productId: string;
}

export default CartItemProps;
