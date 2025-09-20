import { createContext, useContext, useState, ReactNode } from 'react';

export type InfoMsgType = {
    msg: string;
    type: 'info' | 'error';
    id: string;
};

export type InfoMsgToAddType = {
    msg: string;
    type: 'info' | 'error';
};

type InfoMsgContextType = {
    state: InfoMsgType[];
    removeItem: (id: string) => void;
    addItem: (infoMsg: InfoMsgToAddType) => void;
};

const GlobalStateContext = createContext<InfoMsgContextType | undefined>(undefined);

type InfoMsgProviderProps = {
    children: ReactNode;
};

export function InfoMsgProvider({ children }: InfoMsgProviderProps) {
    const [infoMsgs, setInfoMsgs] = useState<InfoMsgType[]>([]);

    const onAddItem = (infoMsg: InfoMsgToAddType) => {
        const id = crypto.randomUUID();

        setInfoMsgs((prev) => [...prev, { msg: infoMsg.msg, type: infoMsg.type, id }]);
    };

    const onRemoveItem = (id: string) => {
        setInfoMsgs((prev) => prev.filter((item) => item.id !== id));
    };

    return (
        <GlobalStateContext.Provider value={{ state: infoMsgs, removeItem: onRemoveItem, addItem: onAddItem }}>
            {children}
        </GlobalStateContext.Provider>
    );
}

export function useInfoMsg() {
    const context = useContext(GlobalStateContext);
    if (!context) throw new Error('useInfoMsg must be used within InfoMsgProvider');
    return context;
}
