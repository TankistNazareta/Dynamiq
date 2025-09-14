import { useState } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import { CouponRes, getCoupon } from '../../services/client/coupon';
import useHttpHook from '../../hooks/useHttp';
import { ErrorMsgType } from '../../utils/types/api';

interface PopupCouponProps {
    show: boolean;
    onClose: () => void;
    onApply: (coupon: CouponRes) => void;
}

const PopupCoupon: React.FC<PopupCouponProps> = ({ show, onClose, onApply }) => {
    const [coupon, setCoupon] = useState('');
    const [error, setError] = useState('');

    const { state, setState, makeRequest } = useHttpHook();

    const onCheckCoupon = (coupon: string) => {
        makeRequest<CouponRes>(() => getCoupon(coupon))
            .then((res) => {
                onApply(res);
                setState('success');
                setCoupon('');
                onClose();
            })
            .catch((error: ErrorMsgType) => {
                setError(error.Message);
            });
    };

    return state === 'loading' ? (
        <Modal show={show} onHide={onClose} centered>
            <Modal.Header>
                <Modal.Title>Checking Coupon</Modal.Title>
            </Modal.Header>
            <Modal.Body className="text-center">
                <div className="d-flex flex-column align-items-center">
                    <div className="spinner-border text-primary mb-3" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                    <p>Please wait while we check your coupon.</p>
                </div>
            </Modal.Body>
        </Modal>
    ) : (
        <Modal show={show} onHide={onClose} centered>
            <Modal.Header closeButton>
                <Modal.Title>Enter Coupon</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form.Group>
                    <Form.Label>Coupon Code</Form.Label>
                    <Form.Control
                        type="text"
                        value={coupon}
                        onChange={(e) => setCoupon(e.target.value)}
                        isInvalid={!!error}
                    />
                    <Form.Control.Feedback type="invalid">{error}</Form.Control.Feedback>
                </Form.Group>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={onClose}>
                    Cancel
                </Button>
                <Button variant="primary" onClick={() => onCheckCoupon(coupon)}>
                    Apply
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default PopupCoupon;
