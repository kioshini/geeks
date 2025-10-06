import WebApp from '@twa-dev/sdk';

export type TgUser = {
	id: number;
	first_name?: string;
	last_name?: string;
	username?: string;
	language_code?: string;
	is_bot?: boolean;
};

export const Telegram = {
	ready: () => {
		try { WebApp.ready(); } catch {}
	},
	expand: () => {
		try { WebApp.expand(); } catch {}
	},
	theme: () => WebApp.colorScheme,
	user: (): TgUser | undefined => {
		// Preferred source: initDataUnsafe.user
		const u = WebApp.initDataUnsafe?.user as any;
		if (u && typeof u.id === 'number') return u as TgUser;
		return undefined;
	},
	haptic: (type: 'impact' | 'notification' | 'selection' = 'selection') => {
		try {
			if (type === 'impact') WebApp.HapticFeedback.impactOccurred('light');
			else if (type === 'notification') WebApp.HapticFeedback.notificationOccurred('success');
			else WebApp.HapticFeedback.selectionChanged();
		} catch {}
	},
};
