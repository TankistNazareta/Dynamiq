const parseDateTime = (date: Date) => {
    const parseIsoWithHighPrecision = (s?: string | Date | null): Date | null => {
        if (!s) return null;
        if (s instanceof Date) return s;
        const str = String(s);
        if (/[zZ]|[+\-]\d{2}:\d{2}$/.test(str)) {
            const d = new Date(str);
            return Number.isNaN(d.getTime()) ? null : d;
        }
        const withMs = str.replace(/\.(\d{1,7})\d*/, (_m, p) => '.' + (p + '000').slice(0, 3));
        const normalized = withMs.includes('.') ? withMs + 'Z' : withMs + '.000Z';
        const d = new Date(normalized);
        return Number.isNaN(d.getTime()) ? null : d;
    };

    const createdDate = parseIsoWithHighPrecision(date);
    return createdDate
        ? createdDate.toLocaleString('en-GB', {
              month: 'short',
              day: '2-digit',
              hour: '2-digit',
              minute: '2-digit',
              hour12: false,
          })
        : 'Invalid date';
};

export default parseDateTime;
