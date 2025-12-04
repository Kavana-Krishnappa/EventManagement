import eventsReducer, {
  fetchEvents,
  createEvent,
  updateEvent,
  deleteEvent,
} from '../../../features/events/eventsSlice';

jest.mock('../../../api/axiosClient');

describe('eventsSlice', () => {
  const initialState = {
    list: [],
    loading: false,
    error: null,
  };

  describe('reducers', () => {
    it('should return initial state', () => {
      expect(eventsReducer(undefined, { type: 'unknown' })).toEqual(initialState);
    });
  });

  describe('fetchEvents async thunk', () => {
    it('should handle fetchEvents.pending', () => {
      const action = { type: fetchEvents.pending.type };
      const state = eventsReducer(initialState, action);

      expect(state.loading).toBe(true);
      expect(state.error).toBe(null);
    });

    it('should handle fetchEvents.fulfilled', () => {
      const mockEvents = [
        { eventId: 1, eventName: 'Event 1' },
        { eventId: 2, eventName: 'Event 2' },
      ];
      const action = { type: fetchEvents.fulfilled.type, payload: mockEvents };
      const state = eventsReducer(initialState, action);

      expect(state.loading).toBe(false);
      expect(state.list).toEqual(mockEvents);
    });

    it('should handle fetchEvents.rejected', () => {
      const action = {
        type: fetchEvents.rejected.type,
        payload: 'Failed to fetch events',
      };
      const state = eventsReducer(initialState, action);

      expect(state.loading).toBe(false);
      expect(state.error).toBe('Failed to fetch events');
    });
  });

  describe('createEvent async thunk', () => {
    it('should handle createEvent.fulfilled', () => {
      const newEvent = { eventId: 3, eventName: 'New Event' };
      const action = { type: createEvent.fulfilled.type, payload: newEvent };
      const state = eventsReducer(initialState, action);

      expect(state.loading).toBe(false);
      expect(state.list).toContainEqual(newEvent);
    });
  });

  describe('updateEvent async thunk', () => {
    it('should handle updateEvent.fulfilled', () => {
      const stateWithEvents = {
        ...initialState,
        list: [
          { eventId: 1, eventName: 'Event 1', location: 'Old Location' },
          { eventId: 2, eventName: 'Event 2', location: 'Location 2' },
        ],
      };

      const updateData = { location: 'New Location' };
      const action = {
        type: updateEvent.fulfilled.type,
        payload: { id: 1, data: updateData },
      };
      const state = eventsReducer(stateWithEvents, action);

      expect(state.list[0].location).toBe('New Location');
      expect(state.list[1].location).toBe('Location 2');
    });
  });

  describe('deleteEvent async thunk', () => {
    it('should handle deleteEvent.fulfilled', () => {
      const stateWithEvents = {
        ...initialState,
        list: [
          { eventId: 1, eventName: 'Event 1' },
          { eventId: 2, eventName: 'Event 2' },
        ],
      };

      const action = { type: deleteEvent.fulfilled.type, payload: 1 };
      const state = eventsReducer(stateWithEvents, action);

      expect(state.list).toHaveLength(1);
      expect(state.list[0].eventId).toBe(2);
    });
  });
});
