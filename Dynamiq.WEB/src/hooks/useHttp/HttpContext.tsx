import { createContext, useContext, ReactNode } from 'react';
import useHttpHook, { stateType } from './index';

type HttpContextType = {
    makeRequest: <T>(callMethod: () => Promise<any>) => Promise<T>;
    state: stateType;
    setState: (val: stateType) => void;
};

const HttpContext = createContext<HttpContextType | undefined>(undefined);

export function HttpProvider({ children }: { children: ReactNode }) {
    const { makeRequest, state, setState } = useHttpHook();

    return <HttpContext.Provider value={{ makeRequest, state, setState }}>{children}</HttpContext.Provider>;
}

export function useHttp() {
    const context = useContext(HttpContext);
    if (!context) throw new Error('useHttp must be used within HttpProvider');
    return context;
}
