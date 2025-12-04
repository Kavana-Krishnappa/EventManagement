import eventApi from '../../api/eventApi';

jest.mock('../../api/axiosClient');

describe('eventApi', () => {
  let axiosClient;

  beforeEach(() => {
    axiosClient = require('../../api/axiosClient').default;
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  describe('getAll', () => {
    it('should fetch all events', async () => {
      const mockEvents = [{ eventId: 1, eventName: 'Test Event' }];
      axiosClient.get.mockResolvedValue({ data: mockEvents });

      const result = await eventApi.getAll();

      expect(axiosClient.get).toHaveBeenCalledWith('Events/All');
      expect(result.data).toEqual(mockEvents);
    });
  });

  //for await error
  /* eslint-disable testing-library/no-await-sync-query */

  describe('getById', () => {
    it('should fetch event by id', async () => {
      const mockEvent = { eventId: 1, eventName: 'Test Event' };
      axiosClient.get.mockResolvedValue({ data: mockEvent });
      
      const result = await eventApi.getById(1);
      

      expect(axiosClient.get).toHaveBeenCalledWith('/Events/1');
      expect(result.data).toEqual(mockEvent);
    });
  });

  describe('create', () => {
    it('should create a new event', async () => {
      const newEvent = {
        eventName: 'New Event',
        description: 'Description',
        location: 'Location',
        maxCapacity: 100,
        eventDate: '2024-12-31T10:00:00',
      };
      const mockResponse = { data: { ...newEvent, eventId: 1 } };
      axiosClient.post.mockResolvedValue(mockResponse);

      const result = await eventApi.create(newEvent);

      expect(axiosClient.post).toHaveBeenCalledWith('/Events/create', newEvent);
      expect(result.data).toEqual(mockResponse.data);
    });
  });

  describe('update', () => {
    it('should update an event', async () => {
      const updateData = [{ op: 'replace', path: '/eventName', value: 'Updated Event' }];
      axiosClient.patch.mockResolvedValue({ data: {} });

      await eventApi.update(1, updateData);

      expect(axiosClient.patch).toHaveBeenCalledWith('/Events/1', updateData);
    });
  });

  describe('delete', () => {
    it('should delete an event', async () => {
      axiosClient.delete.mockResolvedValue({ data: {} });

      await eventApi.delete(1);

      expect(axiosClient.delete).toHaveBeenCalledWith('/Events/1');
    });
  });

  describe('getCapacity', () => {
    it('should fetch event capacity info', async () => {
      const capacityInfo = {
        currentRegistrations: 50,
        maxCapacity: 100,
        availableSpots: 50,
        isFull: false,
      };
      axiosClient.get.mockResolvedValue({ data: capacityInfo });

      const result = await eventApi.getCapacity(1);

      expect(axiosClient.get).toHaveBeenCalledWith('/Events/1/capacity');
      expect(result.data).toEqual(capacityInfo);
    });
  });
});
